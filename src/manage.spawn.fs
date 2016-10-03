module SpawnManager
open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers

let maxParts (energy: float) =
    // add work, carry, move, move until capacity
    // fill with move (1 extra per carry)
    let baseCost = 250.
    let moveCost = 50.
    let scale = int(Math.Floor(energy / baseCost))
    let extraMoves = int(Math.Floor((energy - baseCost) / moveCost))
    new ResizeArray<string>[|
        for x in 1 .. scale do
            yield Globals.WORK
            yield Globals.CARRY
            yield Globals.MOVE
            yield Globals.MOVE
            for y in 1 .. extraMoves do yield Globals.MOVE
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
        if spawn.energy = spawn.energyCapacity then
            let parts = maxParts(spawn.energy)
            let memory = { controllerId = spawn.room.controller.id; spawnId = spawn.id; role = nextRole(); lastAction = Idle }
            let result = spawn.createCreep(parts, null, box (memory))
            match box result with
            | :? string -> printfn "Spawned creep name: %s" (unbox<string> result)
            | _ -> ()
    | None -> ()