module Repair
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

let repairStructures (creep: Creep) =
    let structure = 
        creep.pos.findClosestByPath(Globals.FIND_STRUCTURES, filter<Structure>(fun s -> s.hits < s.hitsMax))
        // |> Seq.toList
        // |> List.map (fun s -> printfn "look found a: %A" s; s)
        // |> List.filter (fun s -> s.hits < s.hitsMax)
        // |> List.tryHead
    match structure with
    | Some s ->
        // printfn "%s attempting quick repair" creep.name
        match (creep.repair(U2.Case2 s)) with
        | r when r = Globals.OK -> Repairing
        | r -> failwith (sprintf "unhandled error code %f" r)
    | None ->
        Fail

let run(creep: Creep, memory: CreepMemory) =
    let setLastAction action =
        // printfn "%s is %A " creep.name action 
        setCreepMemory creep { memory with lastAction = action }
    let findEnergy() = findEnergy creep |> setLastAction
    let repair () = repairStructures creep |> setLastAction

    match ((creepEnergy creep), memory.lastAction) with
    | (Energy _, Repairing)         -> repair()
    | (Energy _, Moving Repair)     -> repair()
    | (Energy _, Harvesting)        -> findEnergy()
    | (Energy _, Moving Harvest)    -> findEnergy()
    | (Empty, _)                    -> findEnergy()
    | (Full, _)                     -> repair()
    | (_, lastaction)    ->
        // printfn "%s ?? %A" creep.name lastaction 
        findEnergy()
