module Manage.Construction
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

(*
Construction Helpers
- determine walls to be built around the Room
- determine placement of the spawn?
*)
let defaultToString x =
    match x with
    | Some y -> y
    | None -> ""


let pathToTuple (path: PathStep seq) = 
    path |> Seq.map (fun res -> (res.x, res.y))

let buildStuff (room: Room) (structure: string) (path : (float * float) seq) =
    path |> Seq.iter (fun (x, y) -> room.createConstructionSite(x, y, structure) |> printfn "%s result: %f" structure)

/// Goal 1: create walls within 1 square of all 4 edges - DONE
/// Goal 2: create walls on the outer edges that block off exits
/// Goal 3: leave gaps for Ramparts that allow my creeps to pass through
let createOuterWalls (room: Room) =
    let wallPositions = 
        unbox<ResizeArray<LookAtResultWithPos>> (box (room.lookAtArea(0.,0.,49.,49., true)))
        |> Seq.filter (fun res -> 
            res.``type`` = Globals.LOOK_TERRAIN
            && (defaultToString res.terrain) = "plain"
            && (
                (res.x = 4. && res.y >= 4. && res.y <= 45.)
                ||
                (res.x = 45. && res.y >= 4. && res.y <= 45.)
                ||
                (res.y = 4. && res.x >= 4. && res.x <= 45.)
                || 
                (res.y = 45. && res.x >= 4. && res.x <= 45.)
            ))
        |> Seq.map (fun res -> (res.x, res.y))
    //printfn "proposed positions: %A" wallPositions
    wallPositions |> buildStuff room Globals.STRUCTURE_WALL


let createRoadsAroundSpawn (spawn: Spawn) =
    let (x, y) = (spawn.pos.x, spawn.pos.y)
    let adjacentPositions = [
        (x - 1., y - 1.); (x, y - 1.); (x + 1., y - 1.);
        (x - 1., y);  (x + 1., y);
        (x - 1., y + 1.); (x, y + 1.); (x + 1., y + 1.);
        ]
    adjacentPositions |> buildStuff spawn.room Globals.STRUCTURE_ROAD
    ()


/// Goal 1: create roads from each energy source to spawn
/// Goal 2: create roads from spawn to Controller
/// Goal 3: create roads from each energy source to Controller
let createRoadsToSpawn (spawn: Spawn) level =
    match level with
    | 1 ->
        let closestSource = spawn.pos.findClosestByPath<Source>(Globals.FIND_SOURCES_ACTIVE)
        spawn.pos.findPathTo(U2.Case2 (box closestSource)) 
        |> pathToTuple 
        |> buildStuff spawn.room Globals.STRUCTURE_ROAD
    | 2 ->
        spawn.pos.findPathTo(U2.Case2 (box spawn.room.controller)) 
        |> pathToTuple 
        |> buildStuff spawn.room Globals.STRUCTURE_ROAD
    | _ -> ()

    ()

/// Goal 1: wait until level 1 roads have been built
/// Goal 2: place extensions near designated area for extensions
/// Goal 3: utilize a lattice pattern for placement
let createExtensions (room: Room) level =
    match level with
    | 2 ->
        ()
    | _ -> ()

/// build storage containers

/// build towers

/// build ramparts

