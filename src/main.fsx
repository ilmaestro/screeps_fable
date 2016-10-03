#r "../node_modules/fable-core/Fable.Core.dll"
#load "../library/Fable.Import.Screeps.fs"
#load "./helpers.fs"
#load "./action.harvest.fs"
#load "./action.upgrade.fs"
#load "./action.build.fs"
#load "./manage.spawn.fs"

open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers

[<Emit("Object.keys($0)")>]
let getKeys obj: string list = jsNative

let creepDispatcher name =
    match unbox Globals.Game.creeps?(name) with
    | Some c -> 
        let creep = c :> Creep
        let memory = creepMemory creep
        let action =
            match memory.role with
            | Harvest -> Harvest.run
            | Upgrade -> Upgrade.run
            | Build -> Build.run
        action(creep, memory)
    | None -> ()

let loop() =
    getKeys Globals.Game.spawns |> List.iter SpawnManager.run
    getKeys Globals.Game.creeps |> List.iter creepDispatcher