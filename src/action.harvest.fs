module Harvest
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers

let energyStructures = new ResizeArray<string>[|Globals.STRUCTURE_SPAWN; Globals.STRUCTURE_EXTENSION; Globals.STRUCTURE_TOWER; |]
let resourceContainers = new ResizeArray<string>[|Globals.STRUCTURE_CONTAINER; Globals.STRUCTURE_STORAGE; |]

let energyStructureFilter =
    filter<EnergyStructure>(fun s -> 
        energyStructures.Contains(s.structureType) && s.energy < s.energyCapacity)

let resourceContainerNotFull (r: ResourceContainer) =
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

let resourceContainerFilter =
    filter<ResourceContainer>(fun r ->
        resourceContainers.Contains(r.structureType) && resourceContainerNotFull(r))

let findEnergy (creep: Creep) =
    match (creep.pos.findClosestByPath(Globals.FIND_SOURCES_ACTIVE)) with
    | Some target ->
        match (creep.harvest(target)) with
        | r when r = Globals.OK -> 
            Harvesting
        | r when r = Globals.ERR_NOT_IN_RANGE -> 
            creep.moveTo(U2.Case2 (box target)) |> ignore
            Moving (Harvest)
        | _ -> Fail
    | None -> Fail

let transferEnergy (creep: Creep) =
    match (creep.pos.findClosestByPath(Globals.FIND_STRUCTURES, energyStructureFilter)) with
    | Some spawn ->
        match (creep.transfer(spawn, Globals.RESOURCE_ENERGY)) with
        | r when r = Globals.OK -> Transferring
        | r when r = Globals.ERR_NOT_IN_RANGE ->
            creep.moveTo(U2.Case2 (box spawn)) |> ignore
            Moving (Harvest)
        | _ -> Fail
    | None -> Fail

let run(creep: Creep, memory: CreepMemory) =
    let setLastAction action = setCreepMemory creep { memory with lastAction = action }
    let findEnergy() = findEnergy creep |> setLastAction
    let transfer() = transferEnergy creep |> setLastAction

    match ((creepEnergy creep), memory.lastAction) with
    | (Empty, _)    -> findEnergy()
    | (Full, _)     -> transfer() // only takes a single tick to transfer
    | (_, lastaction)    ->
        //printfn "%s ?? %A" creep.name lastaction 
        findEnergy()

