module Model.Domain
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

type Allie = {
    userName: string;
    roomName: string;
}

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
    store: System.Collections.Generic.IDictionary<string,float>;
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
    | Guard
    | Attacker
    | Claimer
    | Pioneer
    | Transport

type CreepAction =
    | Moving of CreepAction // since its not role-specific
    | Harvesting
    | Transferring
    | Upgrading
    | Building of Position
    | Repairing
    | Defending
    | Attacking
    | Claiming
    | Pioneering
    | Idle

type ActionResult =
    | Success of Creep * CreepAction
    | Failure of float

type ConstructionItemStatus =
    | Unconstructed
    | Inprogress
    | Completed

type ConstructionItem = {
    position: Position;
    structureType: string;
    }

type CreepMemory = {
    controllerId: string;
    spawnId: string;
    role: RoleType;
    lastAction: CreepAction;
}

type GameMemory = {
    creepCount: int;
    constructionQueue: ConstructionItem list;
    constructionItem: ConstructionItem option;
}

type SpawnMemory = {
    lastRoleItem: int;
    lastConstructionLevel: int;
}

//let roleOrder = [Harvest; Harvest; Build; Build;]
let roleOrder = [Harvest; Build; Harvest; Upgrade; Repair; Guard;]

let workerTemplate = seq { yield Globals.WORK; yield Globals.CARRY; yield Globals.MOVE; yield Globals.MOVE; }
let transportTemplate = seq { yield Globals.CARRY; yield Globals.MOVE; yield Globals.MOVE; }
let claimerTemplate = seq { yield Globals.MOVE; yield Globals.MOVE; yield Globals.MOVE; yield Globals.MOVE; yield Globals.CLAIM; }
let guardTemplate = seq { yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.MOVE; yield Globals.MOVE; yield Globals.ATTACK; yield Globals.HEAL }
let banditTemplate = seq { yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.MOVE; yield Globals.MOVE; yield Globals.MOVE; yield Globals.MOVE; yield Globals.ATTACK; yield Globals.HEAL }
let alliesList = ResizeArray<string>[| "CaptainSketchy" |]
let myUsername = "gelletto1138"

// ========
// Utility Functions
// =================
[<Emit("new RoomPosition($0, $1, $2)")>]
let roomPosition (x: float, y: float, roomName: string): RoomPosition = jsNative

let filter<'T> (x: 'T -> bool) =
    createObj [
        "filter" ==> x
    ]

[<Emit("Object.keys($0)")>]
let getKeys obj: string list = jsNative