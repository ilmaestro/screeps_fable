#r "../node_modules/fable-core/Fable.Core.dll"
#load "../library/Fable.Import.Screeps.fs"
#load "./model.domain.fs"
#load "./manage.memory.fs"
#load "./manage.construction.fs"
#load "./manage.spawn.fs"
#load "./action.helpers.fs"
#load "./action.harvest.fs"
#load "./action.upgrade.fs"
#load "./action.build.fs"
#load "./action.repair.fs"
#load "./action.guard.fs"

open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain


let creepDispatcher name =
    match unbox Globals.Game.creeps?(name) with
    | Some c -> 
        let creep = c :> Creep
        let memory = Manage.Memory.MemoryInCreep.get creep
        let action =
            match memory.role with
            | Harvest -> Action.Harvest.run
            | Upgrade -> Action.Upgrade.run
            | Build -> Action.Build.run
            | Repair -> Action.Repair.run
            | Guard -> Action.Guard.run
        // printfn "dispatching %s to %A" creep.name memory.role
        action(creep, memory)
    | None -> ()

let spawnDispatcher name =
    match unbox Globals.Game.spawns?(name) with
    | Some spawn ->
        SpawnManager.run(spawn, Manage.Memory.MemoryInSpawn.get spawn)
    | None -> ()

let loop() =
    Manage.Memory.GameTick.checkMemory()
    Manage.Construction.GameTick.run (Manage.Memory.MemoryInGame.get())
    getKeys Globals.Game.spawns |> List.iter spawnDispatcher
    getKeys Globals.Game.creeps |> List.iter creepDispatcher

// Init the game memory if necessary
match unbox Globals.Memory?game with
| Some g -> ()
| None ->
    Manage.Memory.MemoryInGame.set(
        { 
            creepCount = 0
            constructionQueue = []
            constructionItem = None })