module Action.Helpers
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage.Memory

(*
    Private Stuff
*)
let private energyStructures = new ResizeArray<string>[|Globals.STRUCTURE_SPAWN; Globals.STRUCTURE_EXTENSION; Globals.STRUCTURE_TOWER; |]
let private resourceContainers = new ResizeArray<string>[|Globals.STRUCTURE_CONTAINER; Globals.STRUCTURE_STORAGE; |]

[<Emit("_.sum($0.store) < $0.storeCapacity")>]
let private resourceContainerNotFull (r: ResourceContainer): bool = jsNative

let private resourceContainerHasSome (r: ResourceContainer) resourceType =
    r.store.[resourceType] > 0.

let private findClosest<'T> (find: float) (f: obj) (pos: RoomPosition) =
    Some (pos.findClosestByPath<'T>(find, f))

let private findClosestActiveSources pos = 
    findClosest<Source> Globals.FIND_SOURCES_ACTIVE (filter<Source>(fun _ -> true)) pos

let private findClosestEnergyStructure pos = 
    let structureFilter = filter<EnergyStructure>(fun s -> 
        energyStructures.Contains(s.structureType) && s.energy < s.energyCapacity)
    findClosest<Structure> Globals.FIND_STRUCTURES structureFilter pos

let private findClosestEnergyContainer pos = 
    let containerFilter = filter<ResourceContainer>(fun r ->
        resourceContainers.Contains(r.structureType) && resourceContainerNotFull r)
    findClosest<Structure> Globals.FIND_STRUCTURES containerFilter pos

let private findClosestContainerWithSome pos resourceType =
    let containerFilter = filter<ResourceContainer>(fun r ->
        resourceContainers.Contains(r.structureType) && resourceContainerHasSome r resourceType )
    findClosest<Structure> Globals.FIND_STRUCTURES containerFilter pos

(*
    Public Methods
*)

let beginAction (creep: Creep) =
    //printfn "%s beginning action.."
    Success (creep, Idle)

let endAction memory lastresult =
    match lastresult with
    | Success (creep, action) ->
        // printfn "%s is %A " creep.name action 
        MemoryInCreep.set creep { memory with lastAction = action }
    | Failure r -> printfn "Failure code reported: %f" r

(* You can ALWAYS upgrade your controller *)
let upgradeController lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        let memory = (MemoryInCreep.get creep)
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

let withdrawEnergyFromContainer lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match (findClosestContainerWithSome creep.pos Globals.RESOURCE_ENERGY) with
        | Some target ->
            match creep.withdraw(target, Globals.RESOURCE_ENERGY) with
            | r when r = Globals.OK -> Success (creep, Harvesting)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case1 target.pos) |> ignore
                Success (creep, Moving Harvesting)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let transferEnergyToStructures lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match findClosestEnergyStructure creep.pos with
        | Some structure ->
            match (creep.transfer(U3.Case3 structure, Globals.RESOURCE_ENERGY)) with
            | r when r = Globals.OK -> Success (creep, Transferring)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case2 (box structure)) |> ignore
                Success (creep, Moving Transferring)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let transferEnergyToContainers lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match findClosestEnergyContainer creep.pos with
        | Some structure ->
            match (creep.transfer(U3.Case3 structure, Globals.RESOURCE_ENERGY)) with
            | r when r = Globals.OK -> Success (creep, Transferring)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case2 (box structure)) |> ignore
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

let defendHostiles lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match unbox (creep.pos.findClosestByRange<Creep>(Globals.FIND_HOSTILE_CREEPS)) with
        | Some enemy ->
            match creep.attack(enemy) with
            | r when r = Globals.OK -> Success (creep, Defending)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let healFriends lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match unbox (creep.pos.findClosestByRange<Creep>(Globals.FIND_MY_CREEPS), filter<Creep>(fun c -> c.hits < c.hitsMax)) with
        | Some friend ->
            match creep.heal(friend) with
            | r when r = Globals.OK -> Success (creep, Defending)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let patrol lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        // TODO: patrole the outer .. inner? .. wall.
        Success (creep, Idle)
    | result -> result