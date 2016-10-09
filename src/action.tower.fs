module Action.Tower
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain

type TowerCommandResult =
| Action of StructureTower
| NoAction of StructureTower

let rangedAttack (tower: StructureTower) =
    match unbox (tower.pos.findClosestByRange<Creep>(Globals.FIND_HOSTILE_CREEPS)) with
    | Some target ->
        match tower.attack(target) with
        | _ -> Action tower
    | None -> NoAction tower

let rangedHeal lastCommand =
    match lastCommand with
    | Action tower -> NoAction tower
    | NoAction tower ->
        match unbox (tower.pos.findClosestByRange<Creep>(Globals.FIND_MY_CREEPS), filter<Creep>(fun c -> c.hits < c.hitsMax)) with
        | Some creep ->
            match tower.heal(creep) with
            | _ -> Action tower
        | None -> NoAction tower

let findTowers (room: Room) =
    room.find<StructureTower>(Globals.FIND_STRUCTURES, filter<Structure>(fun s -> s.structureType = Globals.STRUCTURE_TOWER))
    |> Seq.iter (rangedAttack >> rangedHeal >> ignore)