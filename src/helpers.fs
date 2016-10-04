module Helpers
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

type Position = {
    x: float;
    y: float;
    roomName: string;
}

type EnergyStructure = {
    structureType: string;
    energy: float;
    energyCapacity: float;
}

type ResourceContainer = {
    structureType: string;
    store: System.Collections.Generic.IDictionary<int,float>;
    storeCapacity: float;
}

type EnergyState =
    | Empty
    | Energy of float
    | Full

type RoleType =
    | Harvest
    | Upgrade
    | Build
    | Repair

type ActionResult =
    | Moving of RoleType // since its not role-specific
    | Harvesting
    | Transferring
    | Upgrading
    | Building of Position
    | Repairing
    | Idle
    | Fail


type CreepMemory = {
    controllerId: string;
    spawnId: string;
    role: RoleType;
    lastAction: ActionResult;
}

type GameMemory = {
    lastRoleItem: int
}

let roleOrder = [Harvest; Upgrade; Build; Repair;]

[<Emit("new RoomPosition($0, $1, $2)")>]
let roomPosition (x: float, y: float, roomName: string): RoomPosition = jsNative

let filter<'T> (x: 'T -> bool) =
    createObj [
        "filter" ==> x
    ]

let getMemory (object) (name: string) defaultValue =
    match unbox object?(name) with
    | Some result -> box result
    | None -> box defaultValue

let creepEnergy (creep: Creep) =
    match creep.carry.energy with
    | Some energy when energy = creep.carryCapacity -> Full
    | Some energy when energy > 0. -> Energy(energy)
    | _ -> Empty

let creepMemory (creep: Creep) =
    let controllerId = unbox<string> (getMemory creep.memory "controllerId" "")
    let spawnId = unbox<string> (getMemory creep.memory "spawnId" "")
    let role = unbox<RoleType> (getMemory creep.memory "role" Harvest)
    let lastAction = unbox<ActionResult> (getMemory creep.memory "lastAction" Idle)
    { controllerId = controllerId; spawnId = spawnId; role = role; lastAction = lastAction }

let gameMemory() =
    let lastRoleItem = unbox<int> (getMemory (Globals.Memory.Item("game")) "lastRoleItem" 0)
    { lastRoleItem = lastRoleItem}

let setGameMemory (memory: GameMemory) =
    Globals.Memory?game <- memory

let setCreepMemory (creep: Creep) (memory: CreepMemory) =
    let {controllerId=c; spawnId=s; role=r; lastAction=la} = memory
    creep.memory?controllerId <- c
    creep.memory?spawnId <- s
    creep.memory?role <- r
    creep.memory?lastAction <- la

[<Emit("Object.keys($0)")>]
let getKeys obj: string list = jsNative

// Init the game memory if necessary
match unbox Globals.Memory?game with
| Some g -> ()
| None ->
    setGameMemory(
        { lastRoleItem = 0; })