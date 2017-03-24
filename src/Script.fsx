#load "spawnrx.fs"

open Spawn

let ob = Observable.ForProcess "test.sh"

ob.Subscribe (
  {new System.IObserver<_> with
    member x.OnNext(t) = printfn "%A" t
    member x.OnError(ex) = printfn "Error %A" ex
    member x.OnCompleted () = printfn "Completed"}) |> ignore
