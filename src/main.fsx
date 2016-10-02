#r "../node_modules/fable-core/Fable.Core.dll"
#load "../library/Fable.Import.Screeps.fs"
//#load "./aa_game.types.fs"
#load "./action.harvest.fs"
#load "./manage.spawn.fs"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

let loop() =
    SpawnManager.run()
    Harvest.run()