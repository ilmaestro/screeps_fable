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
    | Miner
    | NoRole

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

// type ActionResult =
//     | Success of Creep * CreepAction
//     | Failure of float

type ActionResult<'T, 'U> =
    | Success of 'T
    | Failure of 'U
    | Pass of 'T

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
    actionFlag: string option;
    role: RoleType;
    lastAction: CreepAction;
}

type GameMemory = {
    creepCount: int;
}

type SpawnMemory = {
    lastRoleItem: int;
    lastConstructionLevel: int;
    spawnCreepCount: int;
    areHostileCreepsInRoom: bool;
    constructionQueue: ConstructionItem list;
    constructionItem: ConstructionItem option;
}

type FlagMemory = {
    actionRole: RoleType;
    actionCreepCount: int;
    actionRadius: float;
    actionSpawnId: string;
    currentCreepCount: int;
}

type ActionHandler<'T, 'U> = ActionResult<'T,'U> -> ActionResult<'T,'U>

type CreepActionResult = ActionResult<Creep * CreepAction, float>
type CreepActionHandler = ActionHandler<Creep * CreepAction, float>
type SpawnActionResult = ActionResult<Spawn * SpawnMemory, float>
type TowerActionResult = ActionResult<Tower, float>

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

// TODO: stop using roleOrder and switch over to simply having a base set of harvesting/transferring creeps
//      use flags for : upgrading, building and repairing?
//let roleOrder = [Harvest; Harvest; Build; Build;]
let roleOrder = [Harvest; Build; Harvest; Upgrade; Repair;]

// template name, baseCost
let creepTemplates =
    dict [
        ("worker", 250.), seq { 
            yield Globals.WORK; yield Globals.CARRY; 
            yield Globals.MOVE; yield Globals.MOVE; };
        ("miner", 250.), seq {
            yield Globals.WORK; yield Globals.WORK;
            yield Globals.MOVE; };
        ("transport", 150.), seq { 
            yield Globals.CARRY; 
            yield Globals.MOVE; yield Globals.MOVE; };
        ("claimer", 800.), seq {
            yield Globals.MOVE; yield Globals.MOVE; yield Globals.MOVE; yield Globals.MOVE; 
            yield Globals.CLAIM; };
        ("guardBasic", 200.), seq { 
            yield Globals.TOUGH; yield Globals.TOUGH; 
            yield Globals.MOVE; yield Globals.MOVE; 
            yield Globals.ATTACK; };
        ("guardAdvanced", 570.), seq { 
            yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH;
            yield Globals.MOVE; yield Globals.MOVE;
            yield Globals.ATTACK;
            yield Globals.MOVE; yield Globals.MOVE; 
            yield Globals.HEAL; };
        ("attacker", 570.), seq { 
            yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH;
            yield Globals.MOVE; yield Globals.MOVE;
            yield Globals.ATTACK;
            yield Globals.MOVE; yield Globals.MOVE; 
            yield Globals.HEAL; };
        ("archer", 300.), seq { 
            yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH; yield Globals.TOUGH;
            yield Globals.MOVE; yield Globals.MOVE;
            yield Globals.RANGED_ATTACK; };
    ]

let alliesList = ResizeArray<string>[| "CaptainSketchy" |]
let myUsername = "gelletto1138"

// ========
// Utility Functions
// =================
let roleFromString roleName =
    match roleName with
    | "Harvest" -> Some Harvest
    | "Upgrade" -> Some Upgrade
    | "Build" -> Some Build
    | "Repair" -> Some Repair
    | "Guard" -> Some Guard
    | "Attacker" -> Some Attacker
    | "Claimer" -> Some Claimer
    | "Pioneer" -> Some Pioneer
    | "Transport" -> Some Transport
    | "Miner" -> Some Miner
    | "NoRole" -> Some NoRole
    | _ -> None

[<Emit("new RoomPosition($0, $1, $2)")>]
let roomPosition (x: float, y: float, roomName: string): RoomPosition = jsNative

let filter<'T> (x: 'T -> bool) =
    createObj [
        "filter" ==> x
    ]

[<Emit("(!!$0 && Object.keys($0)) || []")>]
let getKeys obj: string list = jsNative

let getFlags () =
    getKeys Globals.Game.flags |> List.map (fun flagName -> unbox<Flag> Globals.Game.flags?(flagName)) 

