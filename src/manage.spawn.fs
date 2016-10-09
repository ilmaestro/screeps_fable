module SpawnManager
open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage.Memory

(*
    TODOs:
    - Don't spawn more than MAX creeps setting
    - don't spawn repair creeps until there are things to repair.. 
    - level 1 vs. level2 etc... creeps?
*)
let maxCreepsAllowed = 10
let partCosts = 
    dict [
        Globals.MOVE, 50.;
        Globals.WORK, 100.;
        Globals.CARRY, 50.;
        Globals.ATTACK, 80.;
        Globals.RANGED_ATTACK, 150.;
        Globals.HEAL, 250.;
        Globals.CLAIM, 600.;
        Globals.TOUGH, 10.;
        ]
let maxParts (energy: float, roleType: RoleType) =
    // add work, carry, move, move until capacity
    // fill with move (1 extra per carry)

    match roleType with
    | Guard ->
        new ResizeArray<string>[| Globals.ATTACK; Globals.MOVE; Globals.MOVE; Globals.TOUGH; Globals.TOUGH; |]
    | _ -> 
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
                if extraMoves > 0 then yield Globals.MOVE // add an extra move if we can.
            |]

let getNextRole lastRole =
    (roleOrder.Item(lastRole), if lastRole < (roleOrder.Length - 1) then lastRole + 1 else 0)

let ifEmptyQueue queue f spawn =
    match queue with
    | [] -> f spawn
    | _ -> spawn


let checkCreeps (memory: SpawnMemory) (spawn: Spawn) = 
    let maxEnergy = spawn.room.energyAvailable = spawn.room.energyCapacityAvailable

    if maxEnergy && MemoryInGame.get().creepCount < maxCreepsAllowed then
        let (nextRole, nextRoleItem) = getNextRole memory.lastRoleItem
        let creepMemory = { controllerId = spawn.room.controller.id; spawnId = spawn.id; role = nextRole; lastAction = Idle }
        let parts = maxParts(spawn.room.energyAvailable, nextRole)
        let result = spawn.createCreep(parts, null, box (creepMemory))
        match box result with
        | :? string -> 
            printfn "Spawned creep name: %s" (unbox<string> result)
            MemoryInSpawn.set spawn {memory with lastRoleItem = nextRoleItem }
        | :? int -> printfn "Failed to spawn with code %A" (box result)
        | _ -> ()
    spawn

let run (spawn: Spawn, memory: SpawnMemory ) =
    spawn
    |> checkCreeps memory
    |> ifEmptyQueue (MemoryInGame.get().constructionQueue) (Manage.Construction.GameTick.checkConstruction memory)
    |> ignore