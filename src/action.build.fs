module Build
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers
open Harvest

(*
    Todos:
    - find energy from alternate sources
    - immediately repair an item that's just been built - X
*)

let build (creep: Creep) =
    match creep.pos.findClosestByPath(Globals.FIND_CONSTRUCTION_SITES) with
    | Some target ->
        match (creep.build(target)) with
        | r when r = Globals.OK -> Building ({ x = target.pos.x; y = target.pos.y; roomName = target.pos.roomName; })
        | r when r = Globals.ERR_NOT_IN_RANGE ->
            creep.moveTo(U2.Case2 (box target)) |> ignore
            Moving (Build)
        | r -> failwith (sprintf "unhandled error code %f" r)
    | None -> Fail

let quickRepair (creep: Creep) (pos: Position) =
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
        | r when r = Globals.OK -> Building (pos)
        | r -> failwith (sprintf "unhandled error code %f" r)
    | None ->
        Fail

let run(creep: Creep, memory: CreepMemory) =
    let setLastAction action =
        // printfn "%s is %A " creep.name action 
        setCreepMemory creep { memory with lastAction = action }
    let findEnergy() = findEnergy creep |> setLastAction
    let build() = build creep |> setLastAction
    let repair (pos: Position) = quickRepair creep pos |> setLastAction

    match ((creepEnergy creep), memory.lastAction) with
    | (Energy _, Building pos)      ->
        // if the thing we were just building is no longer a construction site, attempt a quick repair on any structures that were built
        let thing = creep.room.lookForAt<ConstructionSite>(Globals.LOOK_CONSTRUCTION_SITES, pos.x, pos.y)
        if thing.Count > 0
        then build() 
        else repair(pos)
    | (Energy _, Moving Build)      -> build()
    | (Energy _, Harvesting)        -> findEnergy()
    | (Energy _, Moving Harvest)    -> findEnergy()
    | (Empty, Building pos)         ->
        printfn "%s just finished building at %A" creep.name pos
        findEnergy()
    | (Empty, _)                    -> findEnergy()
    | (Full, _)                     -> build()
    | (_, lastaction)    ->
        // printfn "%s ?? %A" creep.name lastaction 
        findEnergy()
