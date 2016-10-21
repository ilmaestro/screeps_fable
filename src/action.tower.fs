module Action.Tower
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Action.Helpers
let beginTowerAction (tower: Tower): TowerActionResult = 
    Pass tower

let endTowerAction (lastResult: TowerActionResult) =
    match lastResult with
    | Success _ -> ()
    | Pass _ -> ()
    | Failure r -> printfn "Tower action failure code reported: %f" r

let rangedAttack (lastResult: TowerActionResult)  =
    match lastResult with
    | Pass tower ->
        match unbox (tower.pos.findClosestByRange<Creep>(Globals.FIND_HOSTILE_CREEPS, filter<Creep>(fun c -> not (alliesList.Contains(c.owner.username))))) with
        | Some target ->
            match tower.attack(target) with
            | r when r = Globals.OK -> Success tower
            | r -> Failure r
        | None -> Pass tower
    | result -> result

let rangedHeal (lastResult: TowerActionResult) =
    match lastResult with
    | Pass tower ->
        match unbox (tower.pos.findClosestByRange<Creep>(Globals.FIND_MY_CREEPS, filter<Creep>(fun c -> c.hits < c.hitsMax))) with
        | Some creep ->
            match tower.heal(creep) with
            | r when r = Globals.OK -> Success tower
            | r -> Failure r
        | None -> Pass tower
    | result -> result

let rangedRepair (lastResult: TowerActionResult) =
    match lastResult with
    | Pass tower ->
        match unbox (tower.pos.findClosestByRange<Structure>(Globals.FIND_STRUCTURES, filter<Structure>(fun s -> s.hits < s.hitsMax && s.structureType <> Globals.STRUCTURE_WALL))) with
        | Some structure ->
            match tower.repair(U2.Case2 structure) with
            | r when r = Globals.OK -> Success tower
            | r -> Failure r
        | None -> Pass tower
    | result -> result

let run (room: Room) =


    let towerActions (tower: Tower) =
        let halfFull = (tower.energy / tower.energyCapacity) > 0.5  

        beginTowerAction tower
        |> rangedAttack
        |> doWhen halfFull rangedHeal
        |> doWhen halfFull rangedRepair
        |> endTowerAction

    room.find<StructureTower>(Globals.FIND_STRUCTURES, filter<Structure>(fun s -> s.structureType = Globals.STRUCTURE_TOWER))
    |> Seq.iter towerActions
