module Action.Claim
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage.Memory.MemoryInCreep
open Action.Helpers


let primaryTargetRoomName = 
    { x = 40.; 
        y = 14.;
        roomName = "E69S52";}

/// Goal 1: head to the target room and attack everything
let run(creep: Creep, memory: CreepMemory) =
    beginAction creep
    |> locateRoom primaryTargetRoomName
    //|> attackController
    |> claimController
    //|> reserveController
    |> endAction memory
