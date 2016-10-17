module Manage.Memory
open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain

let private getMemoryObject (object) (name: string) defaultValue =
    match unbox object?(name) with
    | Some result -> box result
    | None -> box defaultValue

let private logDelete name =
    printfn "deleting %s" name
    name

module MemoryInGame =
    let get() =
        let creepCount = unbox<int> (getMemoryObject (Globals.Memory.Item("game")) "creepCount" 0)
        let constructionQueue = unbox<ConstructionItem list> (getMemoryObject (Globals.Memory.Item("game")) "constructionQueue" [])
        let constructionItem = unbox<ConstructionItem option> (getMemoryObject (Globals.Memory.Item("game")) "constructionItem" None)

        {
            creepCount = creepCount
            constructionQueue = constructionQueue
            constructionItem = constructionItem }

    let set (memory: GameMemory) =
        Globals.Memory?game <- memory

module MemoryInSpawn =
    let get (spawn: Spawn) =
        { 
            lastRoleItem = unbox<int> (getMemoryObject spawn.memory "lastRoleItem" 0)
            lastConstructionLevel =  unbox<int> (getMemoryObject spawn.memory "lastConstructionLevel" 0)}

    let set (spawn: Spawn) (memory: SpawnMemory) =
        let {lastRoleItem = r; lastConstructionLevel = lcl} = memory
        spawn.memory?lastRoleItem <- r
        spawn.memory?lastConstructionLevel <- lcl

    let getCreepCount (spawn: Spawn) =
        getKeys Globals.Game.creeps
        |> List.map (fun name -> unbox<Creep> (Globals.Game.creeps?(name)))
        |> List.filter (fun c -> (unbox<string> (c.memory?spawnId)) = spawn.id)
        |> List.length

    [<Emit("delete Memory.spawns[$0]")>]
    let deleteSpawnMemory name = jsNative
    let clearDeadSpawnMems (spawnKeys: ResizeArray<string>) =
        // check for dead spawn memories..
        getKeys Globals.Memory.spawns
        |> List.filter (spawnKeys.Contains >> not)
        |> List.iter (logDelete >> deleteSpawnMemory)
        spawnKeys

module MemoryInFlag =
    let get (flag: Flag) = 
        { 
            actionRole = unbox<RoleType> (getMemoryObject flag.memory "actionRole" NoRole)
            actionCreepCount = unbox<int> (getMemoryObject flag.memory "actionCreepCount" 0)
            actionRadius = unbox<float> (getMemoryObject flag.memory "actionRadius" 0.)
            actionSpawnId = unbox<string> (getMemoryObject flag.memory "actionSpawnId" "")
            currentCreepCount = unbox<int> (getMemoryObject flag.memory "currentCreepCount" 0)
            }
    
    let set (flag: Flag) (memory: FlagMemory) =
        flag.memory?actionRole <- memory.actionRole
        flag.memory?actionCreepCount <- memory.actionCreepCount
        flag.memory?actionRadius <- memory.actionRadius
        flag.memory?actionSpawnId <- memory.actionSpawnId
        flag.memory?currentCreepCount <- memory.currentCreepCount

    [<Emit("delete Memory.flags[$0]")>]
    let deleteFlagMemory name = jsNative
    let clearDeadFlagMems (flagKeys: ResizeArray<string>) =
        // check for dead flag memories..
        getKeys Globals.Memory.flags
        |> List.filter (flagKeys.Contains >> not)
        |> List.iter (logDelete >> deleteFlagMemory)
        flagKeys

module MemoryInCreep =
    (*
        Todos:
        - check for and set global priority flags such as:
            - attacked structures
            - ??
    *)
    let get (creep: Creep) =
        let controllerId = unbox<string> (getMemoryObject creep.memory "controllerId" "")
        let spawnId = unbox<string> (getMemoryObject creep.memory "spawnId" "")
        let role = unbox<RoleType> (getMemoryObject creep.memory "role" Harvest)
        let lastAction = unbox<CreepAction> (getMemoryObject creep.memory "lastAction" Idle)
        let actionFlag = unbox<string option> (getMemoryObject creep.memory "actionFlag" None)
        {
            controllerId = controllerId
            spawnId = spawnId
            actionFlag = actionFlag
            role = role
            lastAction = lastAction }

    let set (creep: Creep) (memory: CreepMemory) =
        let {controllerId=c; spawnId=s; role=r; lastAction=la} = memory
        creep.memory?controllerId <- c
        creep.memory?spawnId <- s
        creep.memory?role <- r
        creep.memory?lastAction <- la
        creep.memory?actionFlag <- memory.actionFlag

    let creepEnergy (creep: Creep) =
        match creep.carry.energy with
        | Some energy when energy = creep.carryCapacity -> Full
        | Some energy when energy > 0. -> Energy(energy)
        | _ -> Empty

    [<Emit("delete Memory.creeps[$0]")>]
    let deleteCreepMemory name = jsNative
    let clearDeadCreepMems (creepKeys: ResizeArray<string>) =
        // check for dead creep memories..
        getKeys Globals.Memory.creeps
        |> List.filter (creepKeys.Contains >> not)
        |> List.iter (logDelete >> deleteCreepMemory)
        creepKeys
    
    let setCurrentCreepCount (creepKeys: ResizeArray<string>) =
        MemoryInGame.set({ MemoryInGame.get() with creepCount = creepKeys.Count})
        creepKeys

module ConstructionMemory =
    (*
        Set up a construction Q.  
        1. Init
        2. Add items
        3. Get next item 
    *)
    let create (items: ConstructionItem list) =
        let gameMemory = MemoryInGame.get()
        MemoryInGame.set({ gameMemory with constructionItem = Some items.Head; constructionQueue = items.Tail })

    let enqueue (item: ConstructionItem) =
        let gameMemory = MemoryInGame.get()
        MemoryInGame.set({ gameMemory with constructionQueue = item :: gameMemory.constructionQueue })

    let dequeue () =
        let gameMemory = MemoryInGame.get()
        match gameMemory.constructionQueue with 
        | [] -> 
            MemoryInGame.set({ gameMemory with constructionItem = None })
            None
        | hd :: tail ->
            MemoryInGame.set({ gameMemory with constructionItem = Some hd; constructionQueue = tail })
            Some hd

module GameTick =
    open MemoryInCreep
    open MemoryInSpawn
    open MemoryInFlag
    let checkMemory () =
        (new ResizeArray<string> (getKeys Globals.Game.creeps))
        |> clearDeadCreepMems
        |> setCurrentCreepCount
        |> ignore
        clearDeadSpawnMems (new ResizeArray<string> (getKeys Globals.Game.spawns)) |> ignore
        clearDeadFlagMems (new ResizeArray<string> (getKeys Globals.Game.flags)) |> ignore