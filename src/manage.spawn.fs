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


let beginSpawnAction (spawn: Spawn, spawnMemory: SpawnMemory ): SpawnActionResult = 
    Pass (spawn, spawnMemory)

let endSpawnAction (lastResult: SpawnActionResult) =
    match lastResult with
    | Success _ -> ()
    | Pass _ -> ()
    | Failure r -> printfn "Spawn action failure code reported: %f" r

let getTemplateByName (name: string) (energy: float) =
    let key = 
        creepTemplates.Keys
        |> Seq.filter (fun (template, cost) -> template.StartsWith(name) && cost <= energy) 
        |> Seq.sortByDescending (fun (template, cost) -> cost)
        |> Seq.head
    creepTemplates.[key]

let maxParts (energy: float, roleType: RoleType) =
    let maximizeParts template =
        let baseCost = totalCost template
        let scale = int(Math.Floor(energy / baseCost))
        seq { 1 .. scale }
        |> Seq.collect (fun _ -> template)

    let parts = 
        match roleType with
        | Guard ->      maximizeParts (getTemplateByName "guard" energy)
        | Attacker ->   maximizeParts (getTemplateByName "attacker" energy)
        | Claimer ->    (getTemplateByName "claimer" energy)
        | Transport ->  maximizeParts (getTemplateByName "transport" energy)
        | _ ->          maximizeParts (getTemplateByName "worker" energy)

    new ResizeArray<string> (parts)

// TODO: put these overrides into memory so they can be set in-game.
let attackerOverride = false
let getNextRole lastRole =
    // (Harvest, lastRole)
    // (Claimer, lastRole)
    // (Pioneer, lastRole)
    if attackerOverride then // Check if we're overriding the next creep spawn with an attacker!
        (Attacker, lastRole)
    else 
        (roleOrder.Item(lastRole), if lastRole < (roleOrder.Length - 1) then lastRole + 1 else 0)

let ifEmptyQueue queue f spawn =
    match queue with
    | [] -> f spawn
    | _ -> spawn

let spawnCreep (spawn: Spawn) parts creepMemory =
    if (totalCost parts) <= spawn.room.energyAvailable then
        let result = spawn.createCreep(parts, null, box (creepMemory))
        match box result with
        | :? string -> 
            printfn "Spawned creep name: %s" (unbox<string> result)
            
        | :? int -> printfn "Failed to spawn with code %A" (box result)
        | _ -> ()
    else 
        ()
        // skip

let checkCreeps (lastResult: SpawnActionResult)  =
    match lastResult with
    | Pass (spawn, spawnMemory) ->
        let maxEnergy = spawn.room.energyAvailable = spawn.room.energyCapacityAvailable
        let spawnCreepCount = MemoryInSpawn.getCreepCount spawn

        if (maxEnergy && spawnCreepCount < maxCreepsAllowed) || spawnCreepCount = 0 then
            let (nextRole, nextRoleItem) = 
                if spawnCreepCount = 0 then (Harvest, spawnMemory.lastRoleItem)
                else getNextRole spawnMemory.lastRoleItem
            let creepMemory = { controllerId = spawn.room.controller.id; spawnId = spawn.id; actionFlag = None; role = nextRole; lastAction = Idle; }
            let parts = maxParts(spawn.room.energyAvailable, nextRole)
            spawnCreep spawn parts creepMemory
            MemoryInSpawn.set spawn {spawnMemory with lastRoleItem = nextRoleItem }
            Success (spawn, spawnMemory)
        else 
            Pass (spawn, spawnMemory)
    | result -> result

// Find flags assigned to a given spawn who's creep counts are less than the designated count
let findFlag spawnId = 
    (getKeys Globals.Game.flags)
    |> List.map (fun key -> 
        let flag = unbox<Flag>(Globals.Game.flags?(key))
        let memory = MemoryInFlag.get flag
        (flag, memory))
    |> List.filter (fun (f,m) -> m.actionSpawnId = spawnId && m.currentCreepCount < m.actionCreepCount)
    |> List.tryHead

let checkFlags (lastResult: SpawnActionResult)  =
    match lastResult with
    | Pass (spawn, spawnMemory) ->
        let maxEnergy = spawn.room.energyAvailable = spawn.room.energyCapacityAvailable
        if maxEnergy then
            // get flags for this spawn that need creeps
            match findFlag spawn.id with
            | Some (flag, flagMemory) ->
                let creepRole = flagMemory.actionRole
                let creepMemory = { controllerId = spawn.room.controller.id; spawnId = spawn.id; actionFlag = Some flag.name; role = creepRole; lastAction = Idle; }
                let parts = maxParts(spawn.room.energyAvailable, creepRole)
                spawnCreep spawn parts creepMemory
                Success (spawn, spawnMemory)
            | None -> Pass (spawn, spawnMemory)
        else Pass (spawn, spawnMemory)   
    | result -> result

let run (spawn: Spawn, memory: SpawnMemory ) =
    beginSpawnAction (spawn, memory)
    |> checkFlags
    |> checkCreeps
    //TODO: Move memory to Spawn... 
    //|> ifEmptyQueue (MemoryInGame.get().constructionQueue) (Manage.Construction.GameTick.checkConstruction memory)
    |> endSpawnAction