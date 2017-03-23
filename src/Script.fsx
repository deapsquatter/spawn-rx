#load "spawnrx.fs"

open spawnrx

let ob = Observable.forProcess "test.sh" Seq.empty

ob.Subscribe (
  {new System.IObserver<_> with
    member x.OnNext(t) = printfn "%A" t
    member x.OnError(ex) = printfn "Error %A" ex
    member x.OnCompleted () = printfn "Completed"}) |> ignore
