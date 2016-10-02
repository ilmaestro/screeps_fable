module SpawnManager
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

let run(name: string) =
    match unbox Globals.Game.spawns?(name) with
    | Some s ->
        let spawn = s :> Spawn 
        if spawn.energy >= 250. then
            let parts = new ResizeArray<string>[|Globals.WORK; Globals.CARRY; Globals.MOVE; Globals.MOVE|]
            let result = spawn.createCreep(parts)
            match box result with
            | :? string -> printfn "Spawned creep name: %s" (unbox<string> result)
            | _ -> ()
    | None -> ()