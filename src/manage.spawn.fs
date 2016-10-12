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
let maxCreepsAllowed = 12
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
let totalCost parts =
    parts |> Seq.map (fun p -> partCosts.[p]) |> Seq.sum

let maxParts (energy: float, roleType: RoleType) =
    let maximizeParts template =
        let baseCost = totalCost template
        let scale = int(Math.Floor(energy / baseCost))
        seq { 1 .. scale }
        |> Seq.collect (fun _ -> template)

    let parts = 
        match roleType with
        | Guard -> maximizeParts guardTemplate
        | Attacker -> maximizeParts banditTemplate
        | Claimer -> maximizeParts claimerTemplate
        | _ -> maximizeParts workerTemplate

    new ResizeArray<string> (parts)

// TODO: put these overrides into memory so they can be set in-game.
let attackerOverride = false
let getNextRole lastRole =
//    (Claimer, lastRole)
//    (Pioneer, lastRole)
    if attackerOverride then // Check if we're overriding the next creep spawn with an attacker!
        (Attacker, lastRole)
    else 
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