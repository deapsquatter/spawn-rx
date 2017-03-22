open System
open System.Diagnostics

let setEnvironmentVariables (startInfo : ProcessStartInfo) environmentSettings =
  for key, value in environmentSettings do
    if startInfo.EnvironmentVariables.ContainsKey key
    then startInfo.EnvironmentVariables.[key] <- value
    else startInfo.EnvironmentVariables.Add(key, value)

module Observable =

  let forProcess fileName environmentSettings =
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
                    |_ -> o.OnCompleted())
        p.OutputDataReceived.Add (fun t -> o.OnNext(Option.ofObj t.Data,None))
        p.ErrorDataReceived.Add (fun t -> o.OnNext(None,Option.ofObj t.Data))
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        p.StandardInput.Close()
        {new IDisposable with
          member x.Dispose () =
            p.Kill()
            p.Dispose() }}
    |> Observable.filter (function |None,None -> false |_ -> true)