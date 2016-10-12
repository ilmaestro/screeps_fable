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
        match unbox (creep.pos.findClosestByPath<Structure>(Globals.FIND_STRUCTURES, filter<Structure>(fun s -> s.hits < s.hitsMax && s.structureType <> Globals.STRUCTURE_WALL))) with
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

/// Goal 1: repair walls wth hits under 5000
/// Goal 2: the minimum hit goes up with the controller level
let repairWalls lastresult =
    let minHits = 5000.
    match lastresult with
    | Success (creep, Idle) ->
        match unbox (creep.pos.findClosestByPath<Structure>(Globals.FIND_STRUCTURES, filter<Structure>(fun s -> s.hits < minHits && s.structureType = Globals.STRUCTURE_WALL))) with
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
        match unbox (creep.pos.findClosestByPath<Creep>(Globals.FIND_HOSTILE_CREEPS, filter<Creep>(fun c -> not (alliesList.Contains(c.owner.username))))) with
        | Some enemy ->
            match creep.attack(enemy) with
            | r when r = Globals.OK -> Success (creep, Defending)
            | r when r = Globals.ERR_NO_BODYPART ->
                // TODO: figure out how to fall back on another body
                Success (creep, Defending)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case2 (box enemy)) |> ignore
                Success (creep, Moving Defending)
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
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case1 friend.pos) |> ignore
                Success (creep, Moving Defending)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let patrol lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        // TODO: patrole the outer .. inner? .. wall.
        match (getKeys Globals.Game.flags |> List.filter (fun name -> name.StartsWith("Guard")) |> List.tryHead) with
        | Some flagName ->
            let flag = unbox<Flag> Globals.Game.flags?(flagName)
            creep.moveTo(U2.Case1 flag.pos) |> ignore
            Success(creep, Moving Defending)
        | None -> Success (creep, Idle)
    | result -> result

let locateRoom targetRoom lastresult =
    let withinBounds (x, y) =
        x < 43. && x > 2. && y > 2. && y < 43. 
    match lastresult with
    | Success (creep, Idle) ->
        if creep.room.name = targetRoom.roomName && withinBounds (creep.pos.x, creep.pos.y)
        then 
            Success(creep, Idle)
        else
            match creep.moveByPath(U3.Case1 (creep.pos.findPathTo(U2.Case1 (roomPosition(targetRoom.x, targetRoom.y, targetRoom.roomName))))) with
            | r when r = Globals.OK -> Success(creep, Moving Attacking)
            | r -> Failure r
    | result -> result

let attackHostileCreeps lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match unbox (creep.pos.findClosestByPath<Creep>(Globals.FIND_HOSTILE_CREEPS, filter<Creep>(fun c -> not (alliesList.Contains(c.owner.username))))) with
        | Some enemy ->
            match creep.attack(enemy) with
            | r when r = Globals.OK -> Success (creep, Defending)
            | r when r = Globals.ERR_NO_BODYPART ->
                // TODO: figure out how to fall back on another body
                Success (creep, Defending)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case2 (box enemy)) |> ignore
                Success (creep, Moving Defending)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let attackHostileStructures lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        match unbox (creep.pos.findClosestByPath<Structure>(Globals.FIND_HOSTILE_STRUCTURES, filter<Structure>(fun s -> s.structureType <> Globals.STRUCTURE_WALL))) with
        | Some enemy ->
            match creep.attack(enemy) with
            | r when r = Globals.OK -> Success (creep, Defending)
            | r when r = Globals.ERR_NO_BODYPART ->
                // TODO: figure out how to fall back on another body
                Success (creep, Defending)
            | r when r = Globals.ERR_NOT_IN_RANGE ->
                creep.moveTo(U2.Case2 (box enemy)) |> ignore
                Success (creep, Moving Defending)
            | r -> Failure r
        | None -> Success (creep, Idle)
    | result -> result

let attackController lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        let target = creep.room.controller
        // match (isNull target.owner), target.my with
        // | false, true -> Success(creep, Idle)
        // | false, false -> Success(creep, Idle)
        // | true, _ ->
        match creep.attackController(creep.room.controller) with
        | r when r = Globals.OK -> Success(creep, Claiming)
        | r when r = Globals.ERR_NOT_IN_RANGE ->
            creep.moveTo(U2.Case2 (box target)) |> ignore
            Success (creep, Moving Claiming)
        | r -> Failure r
    | result -> result

let claimController lastresult =
    match lastresult with
    | Success (creep, Idle) ->
        let target = creep.room.controller
        // match (isNull target.owner), target.my with
        // | false, true -> Success(creep, Idle)
        // | false, false -> Success(creep, Idle)
        // | true, _ ->
        match creep.claimController(creep.room.controller) with
        | r when r = Globals.OK -> Success(creep, Claiming)
        | r when r = Globals.ERR_NOT_IN_RANGE ->
            creep.moveTo(U2.Case2 (box target)) |> ignore
            Success (creep, Moving Claiming)
        | r when r = Globals.ERR_GCL_NOT_ENOUGH ->
            // can't claim this yet..
            Success (creep, Idle)
        | r -> Failure r
    | result -> result
