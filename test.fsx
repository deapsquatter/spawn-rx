#load "spawn-rx.fsx"

open ``Spawn-rx``
let obs = Observable.forProcess "test.sh"

obs.Subscribe (
  {new System.IObserver<_> with
    member x.OnNext(t) = printfn "%A" t
    member x.OnError(ex) = printfn "Error %A" ex
    member x.OnCompleted () = printfn "Completed"}) |> ignore