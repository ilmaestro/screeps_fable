module SpawnManager
open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers

(*
    TODOs:
    - Don't spawn more than MAX creeps setting
    - don't spawn repair creeps until there are things to repair.. 
    - level 1 vs. level2 etc... creeps?
*)
let maxCreepsAllowed = 10
let maxParts (energy: float) =
    // add work, carry, move, move until capacity
    // fill with move (1 extra per carry)
    let baseCost = 250.
    let moveCost = 50.
    let scale = int(Math.Floor(energy / baseCost))
    let extraMoves = int(Math.Floor((energy - baseCost * float(scale)) / moveCost))
    new ResizeArray<string>[|
        for x in 1 .. scale do
            yield Globals.WORK
            yield Globals.CARRY
            yield Globals.MOVE
            yield Globals.MOVE
            //for y in 1 .. extraMoves do yield if y % 2 = 0 then Globals.CARRY else Globals.MOVE
        |]

let nextRole() =
    let memory = gameMemory()
    let nextAction = roleOrder.Item(memory.lastRoleItem)
    let nextRoleItem = if memory.lastRoleItem < (roleOrder.Length - 1) then memory.lastRoleItem + 1 else 0
    setGameMemory({ memory with lastRoleItem = nextRoleItem })
    printfn "next role: %A" nextAction
    nextAction

let run(name: string) =
    match unbox Globals.Game.spawns?(name) with
    | Some s ->
        let spawn = s :> Spawn
        let maxEnergy = spawn.room.energyAvailable = spawn.room.energyCapacityAvailable

        if maxEnergy && gameMemory().creepCount < maxCreepsAllowed then
            let parts = maxParts(spawn.room.energyAvailable)
            let memory = { controllerId = spawn.room.controller.id; spawnId = spawn.id; role = nextRole(); lastAction = Idle }
            let result = spawn.createCreep(parts, null, box (memory))
            match box result with
            | :? string -> printfn "Spawned creep name: %s" (unbox<string> result)
            | :? int -> printfn "Failed to spawn with code %A" (box result)
            | _ -> ()
    | None -> ()