module Action.Helpers
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers

(*
    Types
*)
type CreepRail = Creep -> ActionResult
type CreepContinue = ActionResult -> ActionResult



(*
    Private Stuff
*)
let private energyStructures = new ResizeArray<string>[|Globals.STRUCTURE_SPAWN; Globals.STRUCTURE_EXTENSION; Globals.STRUCTURE_TOWER; |]
let private resourceContainers = new ResizeArray<string>[|Globals.STRUCTURE_CONTAINER; Globals.STRUCTURE_STORAGE; |]

let private energyStructureFilter =
    filter<EnergyStructure>(fun s -> 
        energyStructures.Contains(s.structureType) && s.energy < s.energyCapacity)

let private resourceContainerNotFull (r: ResourceContainer) =
    if r.store.Count = 0 
    then false
    else
        let totalStorage =
            r.store.Keys
            |> Seq.toList 
            |> List.map (fun resourceType -> r.store.[resourceType])
            |> List.append [ 0.; ] // in no keys
            |> List.sum
        totalStorage < r.storeCapacity

let private resourceContainerFilter =
    filter<ResourceContainer>(fun r ->
        resourceContainers.Contains(r.structureType) && resourceContainerNotFull(r))

(*
    Public Methods
*)
let tryOnIdle action creep actionresult =
    match actionresult with
    | Idle -> action creep
    | result -> result

let findClosest<'T> (find: float) (f: obj) (pos: RoomPosition) =
    Some (pos.findClosestByPath<'T>(find, f))

let findClosestStorage pos = 
    findClosest<Structure> Globals.FIND_STRUCTURES resourceContainerFilter pos

let findClosestActiveSources pos = 
    findClosest<Source> Globals.FIND_SOURCES_ACTIVE (filter<Source>(fun _ -> true)) pos

let beginAction (creep: Creep) =
    //printfn "%s beginning action.."
    Success (creep, Idle)

let endAction memory lastresult =
    match lastresult with
    | Success (creep, action) ->
        // printfn "%s is %A " creep.name action 
        setCreepMemory creep { memory with lastAction = action }
    | Failure r -> printfn "Failure code reported: %f" r

(* You can ALWAYS upgrade your controller *)
let upgradeController lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        let memory = (creepMemory creep)
        let controller = unbox<Controller> (Globals.Game.getObjectById(memory.controllerId))
        match (creep.upgradeController(controller)) with
        | r when r = Globals.OK -> Success (creep, Upgrading)
        | r when r = Globals.ERR_NOT_IN_RANGE ->
            creep.moveTo(U2.Case2 (box controller)) |> ignore
            Success (creep, Moving (Upgrading))
        | r -> Failure r
    | result -> result

let pickupDroppedResources lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match findClosest<Resource> Globals.FIND_DROPPED_RESOURCES (filter<Source>(fun _ -> true)) creep.pos with
        | Some target ->
            match creep.pickup(target) with
            | r when r = Globals.OK -> Success (creep, Harvesting)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case1 target.pos) |> ignore
                Success (creep, Moving Harvesting)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let harvestEnergySources lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match findClosestActiveSources creep.pos with
        | Some target ->
            match creep.harvest(U2.Case1 target) with
            | r when r = Globals.OK -> Success (creep, Harvesting)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case1 target.pos) |> ignore
                Success (creep, Moving Harvesting)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

// let findEnergyContainers (creep: Creep) =
//     match findClosestActiveSources creep.pos with
//     | Some target ->
//         match (tryOrMoveTo (creep.withdraw(U2.Case1 target)) creep (U2.Case2 (box target))) with
//         | ActionSuccess _ -> Harvesting
//         | MoveSuccess _ -> Moving Harvesting
//         | Failure _ -> Fail
//     | None -> Fail

let transferEnergy lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match (creep.pos.findClosestByPath(Globals.FIND_STRUCTURES, energyStructureFilter)) with
        | Some spawn ->
            match (creep.transfer(spawn, Globals.RESOURCE_ENERGY)) with
            | r when r = Globals.OK -> Success (creep, Transferring)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case2 (box spawn)) |> ignore
                Success (creep, Moving Transferring)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let build lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match creep.pos.findClosestByPath(Globals.FIND_CONSTRUCTION_SITES) with
        | Some target ->
            match (creep.build(target)) with
            | r when r = Globals.OK -> Success (creep, Building ({ x = target.pos.x; y = target.pos.y; roomName = target.pos.roomName; }))
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case2 (box target)) |> ignore
                Success (creep, Moving (Building ({ x = target.pos.x; y = target.pos.y; roomName = target.pos.roomName; })))
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

(* TODO: fix this so its a valid CreepAction function *)
let quickRepair lastresult =
    match lastresult with
    | Success (creep, Building pos) ->
        let structure = 
            creep.room.lookForAt<Structure>(Globals.LOOK_STRUCTURES, (U2.Case1 (roomPosition(pos.x, pos.y, pos.roomName))))
            |> Seq.toList
            |> List.map (fun s -> printfn "look found a: %A" s; s)
            |> List.filter (fun s -> s.hits < s.hitsMax)
            |> List.tryHead
        match structure with
        | Some s ->
            printfn "%s attempting quick repair" creep.name
            match (creep.repair(U2.Case2 s)) with
            | r when r = Globals.OK -> Success (creep, Building (pos))
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let repairStructures lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match unbox (creep.pos.findClosestByPath<Structure>(Globals.FIND_STRUCTURES, filter<Structure>(fun s -> s.hits < s.hitsMax))) with
        | Some s ->
            // printfn "%s attempting quick repair" creep.name
            match (creep.repair(U2.Case2 s)) with
            | r when r = Globals.OK -> Success (creep, Repairing)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case2 (box s)) |> ignore
                Success (creep, Moving (Repairing))
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

