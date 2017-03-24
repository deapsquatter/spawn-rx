# Spawn child process as an IObservable
Inspired by Paul Betts [spawn-rx](https://github.com/paulcbetts/spawn-rx) and [FAKE ProcessHelper](https://github.com/fsharp/FAKE/blob/master/src/app/FakeLib/ProcessHelper.fs).
## Example Usage
Concatenate the StdOut into a StringBuilder and get the process exit code.
```csharp
open Spawn

Observable.ForProcess fileName
|> fold (fun ((sb:StringBuilder),ex) sp ->
            match sp.StdOut,ex with
            |Some out,None -> sb.Append(out),sp.ExitCode
            |Some out,Some x -> sb.Append(out),Some x
            |None, Some x -> sb, Some x
            |None, None -> sb, sp.ExitCode)
          (StringBuilder(),None)

>val it: IObservable<StringBuilder * int option>
```

## Using with PAKET
`Spawn-rx` can easily be linked as a single file using the PAKET dependency manager. Simply add the following to your `paket.dependencies` file:
```csharp
group Spawn
   github deapsquatter/Spawn-rx /src/spawnrx.fs
```
and to your projects `paket.references` file:
```csharp
group Spawn
    File:spawnrx.fs
```

