namespace spawnrx

open System
open System.Diagnostics

type SpawnProgress =
  {StdOut: string option
   StdErr: string option
   ExitCode: int option}

module Observable =

  let private setEnvironmentVariables (startInfo : ProcessStartInfo) environmentSettings =
    for key, value in environmentSettings do
      if startInfo.EnvironmentVariables.ContainsKey key
      then startInfo.EnvironmentVariables.[key] <- value
      else startInfo.EnvironmentVariables.Add(key, value)

  let forProcess fileName environmentSettings =
    let ofObj value = match value with null -> None | _ -> Some value
    {new IObservable<_> with
      member x.Subscribe(o) =
        let psi = ProcessStartInfo(fileName)
        do setEnvironmentVariables psi environmentSettings
        psi.UseShellExecute <- false
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.RedirectStandardInput <- true
        let p = Process.Start(psi)
        p.EnableRaisingEvents <- true
        p.Exited.Add
          (fun _ -> p.WaitForExit()
                    match p.ExitCode with
                    |e when e <> 0 -> o.OnError(Exception(string e))
                    |_ -> o.OnNext({StdOut=None;StdErr=None;ExitCode=Some p.ExitCode})
                          o.OnCompleted())
        p.OutputDataReceived.Add (fun t -> o.OnNext({StdOut=ofObj t.Data;StdErr=None;ExitCode=None}))
        p.ErrorDataReceived.Add (fun t -> o.OnNext({StdOut=None;StdErr=ofObj t.Data;ExitCode=None}))
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        p.StandardInput.Close()
        {new IDisposable with
          member x.Dispose () =
            p.Kill()
            p.Dispose() }}