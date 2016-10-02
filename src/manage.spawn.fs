module SpawnManager
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

let run() =
    match unbox Globals.Game.spawns?mySpawn with
    | Some s ->
        let spawn = s :> Spawn 
        if spawn.energy > 250. then
            let parts = new ResizeArray<string>[|Globals.WORK; Globals.CARRY; Globals.MOVE; Globals.MOVE|]
            let result = spawn.createCreep(parts, "bork1")
            match box result with
            | :? string -> printfn "Spawned creep name: %s" (unbox<string> result)
            | _ -> 
            ()
    | None -> ()