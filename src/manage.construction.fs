module Manage.Construction
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage

(*
Construction Helpers
- determine walls to be built around the Room
- determine placement of the spawn?
*)
module Things =
    let defaultToString x =
        match x with
        | Some y -> y
        | None -> ""

    let constructionItemStatus item =
        let pos = roomPosition (item.position.x, item.position.y, item.position.roomName)
        let whatsHere = pos.look()
        let hasConstructionSite = whatsHere |> Seq.exists (fun r -> r.``type`` = "constructionSite")
        let hasStructure = whatsHere |> Seq.exists (fun res -> 
            res.``type`` = "structure"
            && res.structure.IsSome
            && res.structure.Value.structureType = item.structureType)

        //printfn "construction status: %b %b %A" hasConstructionSite hasStructure whatsHere 
        match hasConstructionSite, hasStructure with
        | false, false -> Unconstructed
        | false, true -> Completed
        | true, _ -> Inprogress

    let pathToTuple (path: PathStep seq) = 
        path |> Seq.map (fun res -> (res.x, res.y))

    // let buildStuff (room: Room) (structure: string) (path : (float * float) seq) =
    //     path |> Seq.iter (fun (x, y) -> room.createConstructionSite(x, y, structure) |> printfn "%s result: %f" structure)

    let queueStuff (room: Room) (structure: string) (path : (float * float) seq) =
        path |> Seq.iter (fun (x,y) -> Memory.ConstructionMemory.enqueue { position = {x = x; y = y; roomName = room.name}; structureType = structure;})

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
        wallPositions |> queueStuff room Globals.STRUCTURE_WALL


    let createRoadsAroundSpawn (spawn: Spawn) =
        let (x, y) = (spawn.pos.x, spawn.pos.y)
        let adjacentPositions = [
            (x - 1., y - 1.); (x, y - 1.); (x + 1., y - 1.);
            (x - 1., y);  (x + 1., y);
            (x - 1., y + 1.); (x, y + 1.); (x + 1., y + 1.);
            ]
        adjacentPositions |> queueStuff spawn.room Globals.STRUCTURE_ROAD
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
            |> queueStuff spawn.room Globals.STRUCTURE_ROAD
        | 2 ->
            spawn.pos.findPathTo(U2.Case2 (box spawn.room.controller)) 
            |> pathToTuple 
            |> queueStuff spawn.room Globals.STRUCTURE_ROAD
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

module GameTick =
    let rec handleConstruction item =
        match Things.constructionItemStatus item with
        | Unconstructed ->
            let room = unbox<Room>(Globals.Game.rooms?(item.position.roomName))
            room.createConstructionSite(item.position.x, item.position.y, item.structureType) |> printfn "%s result: %f" item.structureType
        | Completed -> 
            match Memory.ConstructionMemory.dequeue() with
            | Some item -> handleConstruction item
            | None -> () 
        | Inprogress -> ()
    
    let run (gameMemory: GameMemory) =
        // check for items in the queue
        match gameMemory.constructionItem with
        | Some item ->
            // check if it needs a construction site or if it has already been built
            handleConstruction item
        | None ->
            // check if there's anything in the queue
            match Memory.ConstructionMemory.dequeue() with
            | Some item -> handleConstruction item
            | None -> () 