#r "../node_modules/fable-core/Fable.Core.dll"
#load "../library/Fable.Import.Screeps.fs"
//#load "./aa_game.types.fs"
#load "./action.harvest.fs"
#load "./manage.spawn.fs"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

[<Emit("Object.keys($0)")>]
let getKeys obj: string list = jsNative

let loop() =
    getKeys Globals.Game.spawns |> List.iter SpawnManager.run
    getKeys Globals.Game.creeps |> List.iter Harvest.run