namespace Fable.Import
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS

// TODO: figure out how this translates with the ints...
type [<AllowNullLiteral>] RAMPART_HITS_MAXType =
    abstract two: float with get, set
    abstract three: float with get, set
    // abstract 4: float with get, set
    // abstract 5: float with get, set
    // abstract 6: float with get, set
    // abstract 7: float with get, set
    // abstract 8: float with get, set

and [<AllowNullLiteral>] BODYPART_COSTType =
    [<Emit("$0[$1]{{=$2}}")>] abstract Item: part: string -> float with get, set
    abstract move: float with get, set
    abstract work: float with get, set
    abstract attack: float with get, set
    abstract carry: float with get, set
    abstract heal: float with get, set
    abstract ranged_attack: float with get, set
    abstract tough: float with get, set
    abstract claim: float with get, set

and [<AllowNullLiteral>] CONSTRUCTION_COSTType =
    abstract spawn: float with get, set
    abstract extension: float with get, set
    abstract road: float with get, set
    abstract constructedWall: float with get, set
    abstract rampart: float with get, set
    abstract link: float with get, set
    abstract storage: float with get, set
    abstract tower: float with get, set
    abstract observer: float with get, set
    abstract powerSpawn: float with get, set
    abstract extractor: float with get, set
    abstract lab: float with get, set
    abstract terminal: float with get, set
    abstract container: float with get, set

and [<AllowNullLiteral>] CONTROLLER_LEVELSType =
    [<Emit("$0[$1]{{=$2}}")>] abstract Item: level: float -> float with get, set

and [<AllowNullLiteral>] CONTROLLER_STRUCTURESType =
    [<Emit("$0[$1]{{=$2}}")>] abstract Item: structure: string -> obj with get, set

and [<AllowNullLiteral>] CONTROLLER_DOWNGRADEType =
    [<Emit("$0[$1]{{=$2}}")>] abstract Item: level: float -> float with get, set

and [<AllowNullLiteral>] MINERAL_MIN_AMOUNTType =
    abstract H: float with get, set
    abstract O: float with get, set
    abstract L: float with get, set
    abstract K: float with get, set
    abstract Z: float with get, set
    abstract U: float with get, set
    abstract X: float with get, set

and [<AllowNullLiteral>] NUKE_DAMAGEType =
    abstract zero: float with get, set
    abstract one: float with get, set
    abstract four: float with get, set

and [<AllowNullLiteral>] REACTIONSType =
    abstract H: obj with get, set
    abstract O: obj with get, set
    abstract Z: obj with get, set
    abstract L: obj with get, set
    abstract K: obj with get, set
    abstract G: obj with get, set
    abstract U: obj with get, set
    abstract OH: obj with get, set
    abstract X: obj with get, set
    abstract ZK: obj with get, set
    abstract UL: obj with get, set
    abstract LH: obj with get, set
    abstract ZH: obj with get, set
    abstract GH: obj with get, set
    abstract KH: obj with get, set
    abstract UH: obj with get, set
    abstract LO: obj with get, set
    abstract ZO: obj with get, set
    abstract KO: obj with get, set
    abstract UO: obj with get, set
    abstract GO: obj with get, set
    abstract LH2O: obj with get, set
    abstract KH2O: obj with get, set
    abstract ZH2O: obj with get, set
    abstract UH2O: obj with get, set
    abstract GH2O: obj with get, set
    abstract LHO2: obj with get, set
    abstract UHO2: obj with get, set
    abstract KHO2: obj with get, set
    abstract ZHO2: obj with get, set
    abstract GHO2: obj with get, set

and [<AllowNullLiteral>] BOOSTSType =
    abstract work: obj with get, set
    abstract attack: obj with get, set
    abstract ranged_attack: obj with get, set
    abstract heal: obj with get, set
    abstract carry: obj with get, set
    abstract move: obj with get, set
    abstract tough: obj with get, set

and [<AllowNullLiteral>] [<Import("*","ConstructionSite")>] ConstructionSite() =
    inherit RoomObject()
    member __.id with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.my with get(): bool = failwith "JS only" and set(v: bool): unit = failwith "JS only"
    member __.owner with get(): Owner = failwith "JS only" and set(v: Owner): unit = failwith "JS only"
    member __.progress with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.progressTotal with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.structureType with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.remove(): float = failwith "JS only"

and Controller =
    StructureController

and Extension =
    StructureExtension

and KeeperLair =
    StructureKeeperLair

and Lab =
    StructureLab

and Link =
    StructureLink

and Observer =
    StructureObserver

and PowerBank =
    StructurePowerBank

and PowerSpawn =
    StructurePowerSpawn

and Rampart =
    StructureRampart

and Terminal =
    StructureTerminal

and Container =
    StructureContainer

and Tower =
    StructureTower

and [<AllowNullLiteral>] Storage =
    abstract store: StoreDefinition with get, set
    abstract storeCapacity: float with get, set
    abstract transfer: target: Creep * resourceType: string * ?amount: float -> float
    abstract transferEnergy: target: Creep * ?amount: float -> float


and [<AllowNullLiteral>] [<Import("*","Creep")>] Creep() =
    inherit RoomObject()
    member __.body with get(): ResizeArray<BodyPartDefinition> = failwith "JS only" and set(v: ResizeArray<BodyPartDefinition>): unit = failwith "JS only"
    member __.carry with get(): StoreDefinition = failwith "JS only" and set(v: StoreDefinition): unit = failwith "JS only"
    member __.carryCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.fatigue with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.hits with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.hitsMax with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.id with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.memory with get(): obj = failwith "JS only" and set(v: obj): unit = failwith "JS only"
    member __.my with get(): bool = failwith "JS only" and set(v: bool): unit = failwith "JS only"
    member __.name with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.owner with get(): Owner = failwith "JS only" and set(v: Owner): unit = failwith "JS only"
    member __.spawning with get(): bool = failwith "JS only" and set(v: bool): unit = failwith "JS only"
    member __.saying with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.ticksToLive with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.attack(target: U3<Creep, Spawn, Structure>): float = failwith "JS only"
    member __.attackController(target: Structure): float = failwith "JS only"
    member __.build(target: ConstructionSite): float = failwith "JS only"
    member __.cancelOrder(methodName: string): float = failwith "JS only"
    member __.claimController(target: Controller): float = failwith "JS only"
    member __.dismantle(target: U2<Spawn, Structure>): float = failwith "JS only"
    member __.drop(resourceType: string, ?amount: float): float = failwith "JS only"
    member __.getActiveBodyparts(``type``: string): float = failwith "JS only"
    member __.harvest(target: U2<Source, Mineral>): float = failwith "JS only"
    member __.heal(target: Creep): float = failwith "JS only"
    member __.move(direction: float): float = failwith "JS only"
    member __.moveByPath(path: U3<ResizeArray<PathStep>, ResizeArray<RoomPosition>, string>): float = failwith "JS only"
    member __.moveTo(x: float, y: float, ?opts: obj): float = failwith "JS only"
    member __.moveTo(target: U2<RoomPosition, obj>, ?opts: obj): float = failwith "JS only"
    member __.notifyWhenAttacked(enabled: bool): float = failwith "JS only"
    member __.pickup(target: Resource): float = failwith "JS only"
    member __.rangedAttack(target: U3<Creep, Spawn, Structure>): float = failwith "JS only"
    member __.rangedHeal(target: Creep): float = failwith "JS only"
    member __.rangedMassAttack(): float = failwith "JS only"
    member __.repair(target: U2<Spawn, Structure>): float = failwith "JS only"
    member __.reserveController(target: Controller): float = failwith "JS only"
    member __.say(message: string, ?toPublic: bool): float = failwith "JS only"
    member __.suicide(): float = failwith "JS only"
    member __.transfer(target: U3<Creep, Spawn, Structure>, resourceType: string, ?amount: float): float = failwith "JS only"
    member __.upgradeController(target: Controller): float = failwith "JS only"
    member __.withdraw(target: Structure, resourceType: string, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","Flag")>] Flag() =
    inherit RoomObject()
    member __.color with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.memory with get(): obj = failwith "JS only" and set(v: obj): unit = failwith "JS only"
    member __.name with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.secondaryColor with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.remove(): unit = failwith "JS only"
    member __.setColor(color: float, ?secondaryColor: float): float = failwith "JS only"
    member __.setPosition(x: float, y: float): float = failwith "JS only"
    member __.setPosition(pos: U2<RoomPosition, obj>): float = failwith "JS only"

and [<AllowNullLiteral>] Game =
    abstract cpu: CPU with get, set
    abstract creeps: obj with get, set
    abstract flags: obj with get, set
    abstract gcl: GlobalControlLevel with get, set
    abstract map: GameMap with get, set
    abstract market: Market with get, set
    abstract rooms: obj with get, set
    abstract spawns: obj with get, set
    abstract structures: obj with get, set
    abstract constructionSites: obj with get, set
    abstract time: float with get, set
    abstract getObjectById: id: string -> 'T
    abstract notify: message: string * ?groupInterval: float -> unit

and [<AllowNullLiteral>] GlobalControlLevel =
    abstract level: float with get, set
    abstract progress: float with get, set
    abstract progressTotal: float with get, set

and [<AllowNullLiteral>] CPU =
    abstract limit: float with get, set
    abstract tickLimit: float with get, set
    abstract bucket: float with get, set
    abstract getUsed: unit -> float

and [<AllowNullLiteral>] BodyPartDefinition =
    abstract boost: string with get, set
    abstract ``type``: string with get, set
    abstract hits: float with get, set

and [<AllowNullLiteral>] Owner =
    abstract username: string with get, set

and [<AllowNullLiteral>] ReservationDefinition =
    abstract username: string with get, set
    abstract ticksToEnd: float with get, set

and [<AllowNullLiteral>] StoreDefinition =
    [<Emit("$0[$1]{{=$2}}")>] abstract Item: resource: string -> float with get, set
    abstract energy: float option with get, set
    abstract power: float option with get, set

and [<AllowNullLiteral>] LookAtResultWithPos =
    abstract x: float with get, set
    abstract y: float with get, set
    abstract ``type``: string with get, set
    abstract creep: Creep option with get, set
    abstract terrain: string option with get, set
    abstract structure: Structure option with get, set
    abstract flag: Flag option with get, set
    abstract energy: Resource option with get, set
    abstract exit: obj option with get, set
    abstract source: Source option with get, set

and [<AllowNullLiteral>] LookAtResult =
    abstract ``type``: string with get, set
    abstract constructionSite: ConstructionSite option with get, set
    abstract creep: Creep option with get, set
    abstract energy: Resource option with get, set
    abstract exit: obj option with get, set
    abstract flag: Flag option with get, set
    abstract source: Source option with get, set
    abstract structure: Structure option with get, set
    abstract terrain: string option with get, set

and [<AllowNullLiteral>] LookAtResultMatrix =
    [<Emit("$0[$1]{{=$2}}")>] abstract Item: coord: float -> U2<LookAtResultMatrix, ResizeArray<LookAtResult>> with get, set

and [<AllowNullLiteral>] FindPathOpts =
    abstract ignoreCreeps: bool option with get, set
    abstract ignoreDestructibleStructures: bool option with get, set
    abstract ignoreRoads: bool option with get, set
    abstract ignore: U2<ResizeArray<obj>, ResizeArray<RoomPosition>> option with get, set
    abstract avoid: U2<ResizeArray<obj>, ResizeArray<RoomPosition>> option with get, set
    abstract maxOps: float option with get, set
    abstract heuristicWeight: float option with get, set
    abstract serialize: bool option with get, set
    abstract maxRooms: float option with get, set
    abstract costCallback: roomName: string * costMatrix: CostMatrix -> CostMatrix

and [<AllowNullLiteral>] MoveToOpts =
    abstract reusePath: float option with get, set
    abstract serializeMemory: bool option with get, set
    abstract noPathFinding: bool option with get, set

and [<AllowNullLiteral>] PathStep =
    abstract x: float with get, set
    abstract dx: float with get, set
    abstract y: float with get, set
    abstract dy: float with get, set
    abstract direction: float with get, set

and [<AllowNullLiteral>] SurvivalGameInfo =
    abstract score: float with get, set
    abstract timeToWave: float with get, set
    abstract wave: float with get, set

and [<AllowNullLiteral>] [<Import("*","GameMap")>] GameMap() =
    member __.describeExits(roomName: string): obj = failwith "JS only"
    member __.findExit(fromRoom: U2<string, Room>, toRoom: U2<string, Room>): U2<string, float> = failwith "JS only"
    member __.findRoute(fromRoom: U2<string, Room>, toRoom: U2<string, Room>, ?opts: obj): U2<ResizeArray<obj>, float> = failwith "JS only"
    member __.getRoomLinearDistance(roomName1: string, roomName2: string): float = failwith "JS only"
    member __.getTerrainAt(x: float, y: float, roomName: string): string = failwith "JS only"
    member __.getTerrainAt(pos: RoomPosition): string = failwith "JS only"
    member __.isRoomProtected(roomName: string): bool = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","Market")>] Market() =
    member __.incomingTransactions with get(): ResizeArray<Transaction> = failwith "JS only" and set(v: ResizeArray<Transaction>): unit = failwith "JS only"
    member __.outgoingTransactions with get(): ResizeArray<Transaction> = failwith "JS only" and set(v: ResizeArray<Transaction>): unit = failwith "JS only"

and [<AllowNullLiteral>] Transaction =
    abstract transactionId: string with get, set
    abstract time: float with get, set
    abstract sender: obj with get, set
    abstract recipient: obj with get, set
    abstract resourceType: string with get, set
    abstract amount: float with get, set
    abstract from: string with get, set
    abstract ``to``: string with get, set
    abstract description: string with get, set

and [<AllowNullLiteral>] Memory =
    [<Emit("$0[$1]{{=$2}}")>] abstract Item: name: string -> obj with get, set
    abstract creeps: obj with get, set
    abstract flags: obj with get, set
    abstract rooms: obj with get, set
    abstract spawns: obj with get, set

and [<AllowNullLiteral>] [<Import("*","Mineral")>] Mineral() =
    inherit RoomObject()
    member __.prototype with get(): Mineral = failwith "JS only" and set(v: Mineral): unit = failwith "JS only"
    member __.mineralAmount with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.mineralType with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.id with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.ticksToRegeneration with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","Nuke")>] Nuke() =
    inherit RoomObject()
    member __.id with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.launchRoomName with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.timeToLand with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"

and [<AllowNullLiteral>] PathFinder =
    abstract CostMatrix: CostMatrix with get, set
    abstract search: origin: RoomPosition * goal: U2<RoomPosition, obj> * ?opts: PathFinderOpts -> obj
    abstract search: origin: RoomPosition * goal: U2<ResizeArray<RoomPosition>, ResizeArray<obj>> * ?opts: PathFinderOpts -> obj
    abstract ``use``: isEnabled: bool -> obj

and [<AllowNullLiteral>] PathFinderOpts =
    abstract plainCost: float option with get, set
    abstract swampCost: float option with get, set
    abstract flee: bool option with get, set
    abstract maxOps: float option with get, set
    abstract maxRooms: float option with get, set
    abstract heuristicWeight: float option with get, set
    abstract roomCallback: roomName: string -> U2<bool, CostMatrix>

and [<AllowNullLiteral>] CostMatrix =
    [<Emit("new $0($1...)")>] abstract Create: unit -> CostMatrix
    abstract set: x: float * y: float * cost: float -> obj
    abstract get: x: float * y: float -> obj
    abstract clone: unit -> CostMatrix
    abstract serialize: unit -> ResizeArray<float>
    abstract deserialize: ``val``: ResizeArray<float> -> CostMatrix

and [<AllowNullLiteral>] RawMemory =
    abstract get: unit -> string
    abstract set: value: string -> obj

and [<AllowNullLiteral>] [<Import("*","Resource")>] Resource() =
    inherit RoomObject()
    member __.amount with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.id with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.resourceType with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","RoomObject")>] RoomObject() =
    member __.prototype with get(): RoomObject = failwith "JS only" and set(v: RoomObject): unit = failwith "JS only"
    member __.pos with get(): RoomPosition = failwith "JS only" and set(v: RoomPosition): unit = failwith "JS only"
    member __.room with get(): Room = failwith "JS only" and set(v: Room): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","RoomPosition")>] RoomPosition(x: float, y: float, roomName: string) =
    member __.roomName with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.x with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.y with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.createConstructionSite(structureType: string): float = failwith "JS only"
    member __.createFlag(?name: string, ?color: float, ?secondaryColor: float): float = failwith "JS only"
    member __.findClosestByPath(``type``: float, ?opts: obj): 'T = failwith "JS only"
    member __.findClosestByPath(objects: U2<ResizeArray<'T>, ResizeArray<RoomPosition>>, ?opts: obj): 'T = failwith "JS only"
    member __.findClosestByRange(``type``: float, ?opts: obj): 'T = failwith "JS only"
    member __.findClosestByRange(objects: U2<ResizeArray<'T>, ResizeArray<RoomPosition>>, ?opts: obj): 'T = failwith "JS only"
    member __.findInRange(``type``: float, range: float, ?opts: obj): ResizeArray<'T> = failwith "JS only"
    member __.findInRange(objects: U2<ResizeArray<'T>, ResizeArray<RoomPosition>>, range: float, ?opts: obj): ResizeArray<'T> = failwith "JS only"
    member __.findPathTo(x: float, y: float, ?opts: FindPathOpts): ResizeArray<PathStep> = failwith "JS only"
    member __.findPathTo(target: U2<RoomPosition, obj>, ?opts: FindPathOpts): ResizeArray<PathStep> = failwith "JS only"
    member __.getDirectionTo(x: float, y: float): float = failwith "JS only"
    member __.getDirectionTo(target: U2<RoomPosition, obj>): float = failwith "JS only"
    member __.getRangeTo(x: float, y: float): float = failwith "JS only"
    member __.getRangeTo(target: U2<RoomPosition, obj>): float = failwith "JS only"
    member __.inRangeTo(target: U2<RoomPosition, obj>, range: float): bool = failwith "JS only"
    member __.isEqualTo(x: float, y: float): bool = failwith "JS only"
    member __.isEqualTo(target: U2<RoomPosition, obj>): bool = failwith "JS only"
    member __.isNearTo(x: float, y: float): bool = failwith "JS only"
    member __.isNearTo(target: U2<RoomPosition, obj>): bool = failwith "JS only"
    member __.look(): ResizeArray<LookAtResult> = failwith "JS only"
    member __.lookFor(``type``: string): ResizeArray<'T> = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","Room")>] Room() =
    member __.controller with get(): Controller = failwith "JS only" and set(v: Controller): unit = failwith "JS only"
    member __.energyAvailable with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacityAvailable with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.memory with get(): obj = failwith "JS only" and set(v: obj): unit = failwith "JS only"
    member __.mode with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.name with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.storage with get(): StructureStorage = failwith "JS only" and set(v: StructureStorage): unit = failwith "JS only"
    member __.survivalInfo with get(): SurvivalGameInfo = failwith "JS only" and set(v: SurvivalGameInfo): unit = failwith "JS only"
    member __.terminal with get(): Terminal = failwith "JS only" and set(v: Terminal): unit = failwith "JS only"
    member __.createConstructionSite(x: float, y: float, structureType: string): float = failwith "JS only"
    member __.createConstructionSite(pos: U2<RoomPosition, obj>, structureType: string): float = failwith "JS only"
    member __.createFlag(x: float, y: float, ?name: string, ?color: float, ?secondaryColor: float): float = failwith "JS only"
    member __.createFlag(pos: U2<RoomPosition, obj>, ?name: string, ?color: float, ?secondaryColor: float): float = failwith "JS only"
    member __.find(``type``: float, ?opts: obj): ResizeArray<'T> = failwith "JS only"
    member __.findExitTo(room: U2<string, Room>): float = failwith "JS only"
    member __.findPath(fromPos: RoomPosition, toPos: RoomPosition, ?opts: FindPathOpts): ResizeArray<PathStep> = failwith "JS only"
    member __.getPositionAt(x: float, y: float): RoomPosition = failwith "JS only"
    member __.lookAt(x: float, y: float): ResizeArray<LookAtResult> = failwith "JS only"
    member __.lookAt(target: U2<RoomPosition, obj>): ResizeArray<LookAtResult> = failwith "JS only"
    member __.lookAtArea(top: float, left: float, bottom: float, right: float, ?asArray: bool): U2<LookAtResultMatrix, ResizeArray<LookAtResultWithPos>> = failwith "JS only"
    member __.lookForAt(``type``: string, x: float, y: float): ResizeArray<'T> = failwith "JS only"
    member __.lookForAt(``type``: string, target: U2<RoomPosition, obj>): ResizeArray<'T> = failwith "JS only"
    member __.lookForAtArea(``type``: string, top: float, left: float, bottom: float, right: float, ?asArray: bool): U2<LookAtResultMatrix, ResizeArray<LookAtResultWithPos>> = failwith "JS only"
    static member serializePath(path: ResizeArray<PathStep>): string = failwith "JS only"
    static member deserializePath(path: string): ResizeArray<PathStep> = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","Source")>] Source() =
    inherit RoomObject()
    member __.prototype with get(): Source = failwith "JS only" and set(v: Source): unit = failwith "JS only"
    member __.energy with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.id with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.ticksToRegeneration with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","Spawn")>] Spawn() =
    inherit OwnedStructure()
    member __.energy with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.hits with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.hitsMax with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.id with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.memory with get(): obj = failwith "JS only" and set(v: obj): unit = failwith "JS only"
    member __.my with get(): bool = failwith "JS only" and set(v: bool): unit = failwith "JS only"
    member __.name with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.owner with get(): Owner = failwith "JS only" and set(v: Owner): unit = failwith "JS only"
    member __.pos with get(): RoomPosition = failwith "JS only" and set(v: RoomPosition): unit = failwith "JS only"
    member __.room with get(): Room = failwith "JS only" and set(v: Room): unit = failwith "JS only"
    member __.structureType with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.spawning with get(): obj = failwith "JS only" and set(v: obj): unit = failwith "JS only"
    member __.canCreateCreep(body: ResizeArray<string>, ?name: string): float = failwith "JS only"
    member __.createCreep(body: ResizeArray<string>, ?name: string, ?memory: obj): obj = failwith "JS only"
    member __.destroy(): float = failwith "JS only"
    member __.isActive(): bool = failwith "JS only"
    member __.notifyWhenAttacked(enabled: bool): float = failwith "JS only"
    member __.renewCreep(target: Creep): float = failwith "JS only"
    member __.recycleCreep(target: Creep): float = failwith "JS only"
    member __.transferEnergy(target: Creep, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureSpawn")>] StructureSpawn() =
    inherit Spawn()


and [<AllowNullLiteral>] [<Import("*","Structure")>] Structure() =
    inherit RoomObject()
    member __.hits with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.hitsMax with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.id with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.structureType with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.destroy(): float = failwith "JS only"
    member __.isActive(): bool = failwith "JS only"
    member __.notifyWhenAttacked(enabled: bool): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","OwnedStructure")>] OwnedStructure() =
    inherit Structure()
    member __.my with get(): bool = failwith "JS only" and set(v: bool): unit = failwith "JS only"
    member __.owner with get(): Owner = failwith "JS only" and set(v: Owner): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureController")>] StructureController() =
    inherit OwnedStructure()
    member __.level with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.progress with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.progressTotal with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.reservation with get(): ReservationDefinition = failwith "JS only" and set(v: ReservationDefinition): unit = failwith "JS only"
    member __.ticksToDowngrade with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.upgradeBlocked with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.unclaim(): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureExtension")>] StructureExtension() =
    inherit OwnedStructure()
    member __.energy with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.transferEnergy(target: Creep, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureLink")>] StructureLink() =
    inherit OwnedStructure()
    member __.cooldown with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energy with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.transferEnergy(target: U2<Creep, StructureLink>, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureKeeperLair")>] StructureKeeperLair() =
    inherit OwnedStructure()
    member __.ticksToSpawn with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureObserver")>] StructureObserver() =
    inherit OwnedStructure()
    member __.observeRoom(roomName: string): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructurePowerBank")>] StructurePowerBank() =
    inherit OwnedStructure()
    member __.power with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.ticksToDecay with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructurePowerSpawn")>] StructurePowerSpawn() =
    inherit OwnedStructure()
    member __.energy with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.power with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.powerCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.createPowerCreep(name: string): float = failwith "JS only"
    member __.processPower(): float = failwith "JS only"
    member __.transferEnergy(target: Creep, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureRampart")>] StructureRampart() =
    inherit OwnedStructure()
    member __.ticksToDecay with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.isPublic with get(): bool = failwith "JS only" and set(v: bool): unit = failwith "JS only"
    member __.setPublic(isPublic: bool): obj = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureRoad")>] StructureRoad() =
    inherit Structure()
    member __.ticksToDecay with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureStorage")>] StructureStorage() =
    inherit OwnedStructure()
    member __.store with get(): StoreDefinition = failwith "JS only" and set(v: StoreDefinition): unit = failwith "JS only"
    member __.storeCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.transfer(target: Creep, resourceType: string, ?amount: float): float = failwith "JS only"
    member __.transferEnergy(target: Creep, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureTower")>] StructureTower() =
    inherit OwnedStructure()
    member __.energy with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.attack(target: Creep): float = failwith "JS only"
    member __.heal(target: Creep): float = failwith "JS only"
    member __.repair(target: U2<Spawn, Structure>): float = failwith "JS only"
    member __.transferEnergy(target: Creep, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureWall")>] StructureWall() =
    inherit Structure()
    member __.ticksToLive with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureExtractor")>] StructureExtractor() =
    inherit OwnedStructure()


and [<AllowNullLiteral>] [<Import("*","StructureLab")>] StructureLab() =
    inherit OwnedStructure()
    member __.cooldown with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energy with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.mineralAmount with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.mineralType with get(): string = failwith "JS only" and set(v: string): unit = failwith "JS only"
    member __.mineralCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.boostCreep(creep: Creep, ?bodyPartsCount: float): float = failwith "JS only"
    member __.runReaction(lab1: StructureLab, lab2: StructureLab): float = failwith "JS only"
    member __.transfer(target: Creep, resourceType: string, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureTerminal")>] StructureTerminal() =
    inherit OwnedStructure()
    member __.store with get(): obj = failwith "JS only" and set(v: obj): unit = failwith "JS only"
    member __.storeCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.send(resourceType: string, amount: float, destination: string, ?description: string): float = failwith "JS only"
    member __.transfer(target: Creep, resourceType: string, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureContainer")>] StructureContainer() =
    inherit Structure()
    member __.store with get(): obj = failwith "JS only" and set(v: obj): unit = failwith "JS only"
    member __.storeCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.transfer(target: Creep, resourceType: string, ?amount: float): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructureNuker")>] StructureNuker() =
    inherit OwnedStructure()
    member __.energy with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.energyCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.ghodium with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.ghodiumCapacity with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.cooldown with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"
    member __.launchNuke(pos: RoomPosition): float = failwith "JS only"

and [<AllowNullLiteral>] [<Import("*","StructurePortal")>] StructurePortal() =
    inherit Structure()
    member __.destination with get(): RoomPosition = failwith "JS only" and set(v: RoomPosition): unit = failwith "JS only"
    member __.ticksToDecay with get(): float = failwith "JS only" and set(v: float): unit = failwith "JS only"

module Globals =
    let [<Global>] FIND_EXIT_TOP: float = jsNative
    let [<Global>] FIND_EXIT_RIGHT: float = jsNative
    let [<Global>] FIND_EXIT_BOTTOM: float = jsNative
    let [<Global>] FIND_EXIT_LEFT: float = jsNative
    let [<Global>] FIND_EXIT: float = jsNative
    let [<Global>] FIND_CREEPS: float = jsNative
    let [<Global>] FIND_MY_CREEPS: float = jsNative
    let [<Global>] FIND_HOSTILE_CREEPS: float = jsNative
    let [<Global>] FIND_SOURCES_ACTIVE: float = jsNative
    let [<Global>] FIND_SOURCES: float = jsNative
    let [<Global>] FIND_DROPPED_RESOURCES: float = jsNative
    let [<Global>] FIND_DROPPED_ENERGY: float = jsNative
    let [<Global>] FIND_STRUCTURES: float = jsNative
    let [<Global>] FIND_MY_STRUCTURES: float = jsNative
    let [<Global>] FIND_HOSTILE_STRUCTURES: float = jsNative
    let [<Global>] FIND_FLAGS: float = jsNative
    let [<Global>] FIND_CONSTRUCTION_SITES: float = jsNative
    let [<Global>] FIND_MY_CONSTRUCTION_SITES: float = jsNative
    let [<Global>] FIND_HOSTILE_CONSTRUCTION_SITES: float = jsNative
    let [<Global>] FIND_MY_SPAWNS: float = jsNative
    let [<Global>] FIND_HOSTILE_SPAWNS: float = jsNative
    let [<Global>] FIND_MINERALS: float = jsNative
    let [<Global>] TOP: float = jsNative
    let [<Global>] TOP_RIGHT: float = jsNative
    let [<Global>] RIGHT: float = jsNative
    let [<Global>] BOTTOM_RIGHT: float = jsNative
    let [<Global>] BOTTOM: float = jsNative
    let [<Global>] BOTTOM_LEFT: float = jsNative
    let [<Global>] LEFT: float = jsNative
    let [<Global>] TOP_LEFT: float = jsNative
    let [<Global>] OK: float = jsNative
    let [<Global>] ERR_NOT_OWNER: float = jsNative
    let [<Global>] ERR_NO_PATH: float = jsNative
    let [<Global>] ERR_NAME_EXISTS: float = jsNative
    let [<Global>] ERR_BUSY: float = jsNative
    let [<Global>] ERR_NOT_FOUND: float = jsNative
    let [<Global>] ERR_NOT_ENOUGH_RESOURCES: float = jsNative
    let [<Global>] ERR_NOT_ENOUGH_ENERGY: float = jsNative
    let [<Global>] ERR_INVALID_TARGET: float = jsNative
    let [<Global>] ERR_FULL: float = jsNative
    let [<Global>] ERR_NOT_IN_RANGE: float = jsNative
    let [<Global>] ERR_INVALID_ARGS: float = jsNative
    let [<Global>] ERR_TIRED: float = jsNative
    let [<Global>] ERR_NO_BODYPART: float = jsNative
    let [<Global>] ERR_NOT_ENOUGH_EXTENSIONS: float = jsNative
    let [<Global>] ERR_RCL_NOT_ENOUGH: float = jsNative
    let [<Global>] ERR_GCL_NOT_ENOUGH: float = jsNative
    let [<Global>] COLOR_RED: float = jsNative
    let [<Global>] COLOR_PURPLE: float = jsNative
    let [<Global>] COLOR_BLUE: float = jsNative
    let [<Global>] COLOR_CYAN: float = jsNative
    let [<Global>] COLOR_GREEN: float = jsNative
    let [<Global>] COLOR_YELLOW: float = jsNative
    let [<Global>] COLOR_ORANGE: float = jsNative
    let [<Global>] COLOR_BROWN: float = jsNative
    let [<Global>] COLOR_GREY: float = jsNative
    let [<Global>] COLOR_WHITE: float = jsNative
    let [<Global>] COLORS_ALL: ResizeArray<float> = jsNative
    let [<Global>] CREEP_SPAWN_TIME: float = jsNative
    let [<Global>] CREEP_LIFE_TIME: float = jsNative
    let [<Global>] CREEP_CLAIM_LIFE_TIME: float = jsNative
    let [<Global>] CREEP_CORPSE_RATE: float = jsNative
    let [<Global>] OBSTACLE_OBJECT_TYPES: ResizeArray<string> = jsNative
    let [<Global>] ENERGY_REGEN_TIME: float = jsNative
    let [<Global>] ENERGY_DECAY: float = jsNative
    let [<Global>] REPAIR_COST: float = jsNative
    let [<Global>] RAMPART_DECAY_AMOUNT: float = jsNative
    let [<Global>] RAMPART_DECAY_TIME: float = jsNative
    let [<Global>] RAMPART_HITS: float = jsNative
    let [<Global>] RAMPART_HITS_MAX: RAMPART_HITS_MAXType = jsNative
    let [<Global>] SPAWN_HITS: float = jsNative
    let [<Global>] SPAWN_ENERGY_START: float = jsNative
    let [<Global>] SPAWN_ENERGY_CAPACITY: float = jsNative
    let [<Global>] SOURCE_ENERGY_CAPACITY: float = jsNative
    let [<Global>] SOURCE_ENERGY_NEUTRAL_CAPACITY: float = jsNative
    let [<Global>] SOURCE_ENERGY_KEEPER_CAPACITY: float = jsNative
    let [<Global>] WALL_HITS: float = jsNative
    let [<Global>] WALL_HITS_MAX: float = jsNative
    let [<Global>] EXTENSION_HITS: float = jsNative
    let [<Global>] EXTENSION_ENERGY_CAPACITY: float = jsNative
    let [<Global>] ROAD_HITS: float = jsNative
    let [<Global>] ROAD_WEAROUT: float = jsNative
    let [<Global>] ROAD_DECAY_AMOUNT: float = jsNative
    let [<Global>] ROAD_DECAY_TIME: float = jsNative
    let [<Global>] LINK_HITS: float = jsNative
    let [<Global>] LINK_HITS_MAX: float = jsNative
    let [<Global>] LINK_CAPACITY: float = jsNative
    let [<Global>] LINK_COOLDOWN: float = jsNative
    let [<Global>] LINK_LOSS_RATION: float = jsNative
    let [<Global>] STORAGE_CAPACITY: float = jsNative
    let [<Global>] STORAGE_HITS: float = jsNative
    let [<Global>] BODYPART_COST: BODYPART_COSTType = jsNative
    let [<Global>] BODYPARTS_ALL: ResizeArray<string> = jsNative
    let [<Global>] CARRY_CAPACITY: float = jsNative
    let [<Global>] HARVEST_POWER: float = jsNative
    let [<Global>] HARVEST_MINERAL_POWER: float = jsNative
    let [<Global>] REPAIR_POWER: float = jsNative
    let [<Global>] DISMANTLE_POWER: float = jsNative
    let [<Global>] BUILD_POWER: float = jsNative
    let [<Global>] ATTACK_POWER: float = jsNative
    let [<Global>] UPGRADE_CONTROLLER_POWER: float = jsNative
    let [<Global>] RANGED_ATTACK_POWER: float = jsNative
    let [<Global>] HEAL_POWER: float = jsNative
    let [<Global>] RANGED_HEAL_POWER: float = jsNative
    let [<Global>] DISMANTLE_COST: float = jsNative
    let [<Global>] MOVE: string = jsNative
    let [<Global>] WORK: string = jsNative
    let [<Global>] CARRY: string = jsNative
    let [<Global>] ATTACK: string = jsNative
    let [<Global>] RANGED_ATTACK: string = jsNative
    let [<Global>] TOUGH: string = jsNative
    let [<Global>] HEAL: string = jsNative
    let [<Global>] CLAIM: string = jsNative
    let [<Global>] CONSTRUCTION_COST: CONSTRUCTION_COSTType = jsNative
    let [<Global>] CONSTRUCTION_COST_ROAD_SWAMP_RATIO: float = jsNative
    let [<Global>] STRUCTURE_EXTENSION: string = jsNative
    let [<Global>] STRUCTURE_RAMPART: string = jsNative
    let [<Global>] STRUCTURE_ROAD: string = jsNative
    let [<Global>] STRUCTURE_SPAWN: string = jsNative
    let [<Global>] STRUCTURE_LINK: string = jsNative
    let [<Global>] STRUCTURE_WALL: string = jsNative
    let [<Global>] STRUCTURE_KEEPER_LAIR: string = jsNative
    let [<Global>] STRUCTURE_CONTROLLER: string = jsNative
    let [<Global>] STRUCTURE_STORAGE: string = jsNative
    let [<Global>] STRUCTURE_TOWER: string = jsNative
    let [<Global>] STRUCTURE_OBSERVER: string = jsNative
    let [<Global>] STRUCTURE_POWER_BANK: string = jsNative
    let [<Global>] STRUCTURE_POWER_SPAWN: string = jsNative
    let [<Global>] STRUCTURE_EXTRACTOR: string = jsNative
    let [<Global>] STRUCTURE_LAB: string = jsNative
    let [<Global>] STRUCTURE_TERMINAL: string = jsNative
    let [<Global>] STRUCTURE_CONTAINER: string = jsNative
    let [<Global>] STRUCTURE_NUKER: string = jsNative
    let [<Global>] STRUCTURE_PORTAL: string = jsNative
    let [<Global>] RESOURCE_ENERGY: string = jsNative
    let [<Global>] RESOURCE_POWER: string = jsNative
    let [<Global>] RESOURCE_UTRIUM: string = jsNative
    let [<Global>] RESOURCE_LEMERGIUM: string = jsNative
    let [<Global>] RESOURCE_KEANIUM: string = jsNative
    let [<Global>] RESOURCE_GHODIUM: string = jsNative
    let [<Global>] RESOURCE_ZYNTHIUM: string = jsNative
    let [<Global>] RESOURCE_OXYGEN: string = jsNative
    let [<Global>] RESOURCE_HYDROGEN: string = jsNative
    let [<Global>] RESOURCE_CATALYST: string = jsNative
    let [<Global>] RESOURCE_HYDROXIDE: string = jsNative
    let [<Global>] RESOURCE_ZYNTHIUM_KEANITE: string = jsNative
    let [<Global>] RESOURCE_UTRIUM_LEMERGITE: string = jsNative
    let [<Global>] RESOURCE_UTRIUM_HYDRIDE: string = jsNative
    let [<Global>] RESOURCE_UTRIUM_OXIDE: string = jsNative
    let [<Global>] RESOURCE_KEANIUM_HYDRIDE: string = jsNative
    let [<Global>] RESOURCE_KEANIUM_OXIDE: string = jsNative
    let [<Global>] RESOURCE_LEMERGIUM_HYDRIDE: string = jsNative
    let [<Global>] RESOURCE_LEMERGIUM_OXIDE: string = jsNative
    let [<Global>] RESOURCE_ZYNTHIUM_HYDRIDE: string = jsNative
    let [<Global>] RESOURCE_ZYNTHIUM_OXIDE: string = jsNative
    let [<Global>] RESOURCE_GHODIUM_HYDRIDE: string = jsNative
    let [<Global>] RESOURCE_GHODIUM_OXIDE: string = jsNative
    let [<Global>] RESOURCE_UTRIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_UTRIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_KEANIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_KEANIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_LEMERGIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_LEMERGIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_ZYNTHIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_ZYNTHIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_GHODIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_GHODIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_UTRIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_UTRIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_KEANIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_KEANIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_LEMERGIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_LEMERGIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_ZYNTHIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_ZYNTHIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_GHODIUM_ACID: string = jsNative
    let [<Global>] RESOURCE_CATALYZED_GHODIUM_ALKALIDE: string = jsNative
    let [<Global>] RESOURCES_ALL: ResizeArray<string> = jsNative
    let [<Global>] CONTROLLER_LEVELS: CONTROLLER_LEVELSType = jsNative
    let [<Global>] CONTROLLER_STRUCTURES: CONTROLLER_STRUCTURESType = jsNative
    let [<Global>] CONTROLLER_DOWNGRADE: CONTROLLER_DOWNGRADEType = jsNative
    let [<Global>] CONTROLLER_CLAIM_DOWNGRADE: float = jsNative
    let [<Global>] CONTROLLER_RESERVE: float = jsNative
    let [<Global>] CONTROLLER_RESERVE_MAX: float = jsNative
    let [<Global>] CONTROLLER_MAX_UPGRADE_PER_TICK: float = jsNative
    let [<Global>] CONTROLLER_ATTACK_BLOCKED_UPGRADE: float = jsNative
    let [<Global>] TOWER_HITS: float = jsNative
    let [<Global>] TOWER_CAPACITY: float = jsNative
    let [<Global>] TOWER_ENERGY_COST: float = jsNative
    let [<Global>] TOWER_POWER_ATTACK: float = jsNative
    let [<Global>] TOWER_POWER_HEAL: float = jsNative
    let [<Global>] TOWER_POWER_REPAIR: float = jsNative
    let [<Global>] TOWER_OPTIMAL_RANGE: float = jsNative
    let [<Global>] TOWER_FALLOFF_RANGE: float = jsNative
    let [<Global>] TOWER_FALLOFF: float = jsNative
    let [<Global>] OBSERVER_HITS: float = jsNative
    let [<Global>] OBSERVER_RANGE: float = jsNative
    let [<Global>] POWER_BANK_HITS: float = jsNative
    let [<Global>] POWER_BANK_CAPACITY_MAX: float = jsNative
    let [<Global>] POWER_BANK_CAPACITY_MIN: float = jsNative
    let [<Global>] POWER_BANK_CAPACITY_CRIT: float = jsNative
    let [<Global>] POWER_BANK_DECAY: float = jsNative
    let [<Global>] POWER_BANK_HIT_BACK: float = jsNative
    let [<Global>] POWER_SPAWN_HITS: float = jsNative
    let [<Global>] POWER_SPAWN_ENERGY_CAPACITY: float = jsNative
    let [<Global>] POWER_SPAWN_POWER_CAPACITY: float = jsNative
    let [<Global>] POWER_SPAWN_ENERGY_RATIO: float = jsNative
    let [<Global>] EXTRACTOR_HITS: float = jsNative
    let [<Global>] LAB_HITS: float = jsNative
    let [<Global>] LAB_MINERAL_CAPACITY: float = jsNative
    let [<Global>] LAB_ENERGY_CAPACITY: float = jsNative
    let [<Global>] LAB_BOOST_ENERGY: float = jsNative
    let [<Global>] LAB_BOOST_MINERAL: float = jsNative
    let [<Global>] LAB_COOLDOWN: float = jsNative
    let [<Global>] GCL_POW: float = jsNative
    let [<Global>] GCL_MULTIPLY: float = jsNative
    let [<Global>] GCL_NOVICE: float = jsNative
    let [<Global>] MODE_SIMULATION: string = jsNative
    let [<Global>] MODE_SURVIVAL: string = jsNative
    let [<Global>] MODE_WORLD: string = jsNative
    let [<Global>] MODE_ARENA: string = jsNative
    let [<Global>] TERRAIN_MASK_WALL: float = jsNative
    let [<Global>] TERRAIN_MASK_SWAMP: float = jsNative
    let [<Global>] TERRAIN_MASK_LAVA: float = jsNative
    let [<Global>] MAX_CONSTRUCTION_SITES: float = jsNative
    let [<Global>] MAX_CREEP_SIZE: float = jsNative
    let [<Global>] MINERAL_REGEN_TIME: float = jsNative
    let [<Global>] MINERAL_MIN_AMOUNT: MINERAL_MIN_AMOUNTType = jsNative
    let [<Global>] MINERAL_RANDOM_FACTOR: float = jsNative
    let [<Global>] TERMINAL_CAPACITY: float = jsNative
    let [<Global>] TERMINAL_HITS: float = jsNative
    let [<Global>] TERMINAL_SEND_COST: float = jsNative
    let [<Global>] TERMINAL_MIN_SEND: float = jsNative
    let [<Global>] CONTAINER_HITS: float = jsNative
    let [<Global>] CONTAINER_CAPACITY: float = jsNative
    let [<Global>] CONTAINER_DECAY: float = jsNative
    let [<Global>] CONTAINER_DECAY_TIME: float = jsNative
    let [<Global>] CONTAINER_DECAY_TIME_OWNED: float = jsNative
    let [<Global>] NUKER_HITS: float = jsNative
    let [<Global>] NUKER_COOLDOWN: float = jsNative
    let [<Global>] NUKER_ENERGY_CAPACITY: float = jsNative
    let [<Global>] NUKER_GHODIUM_CAPACITY: float = jsNative
    let [<Global>] NUKE_LAND_TIME: float = jsNative
    let [<Global>] NUKE_RANGE: float = jsNative
    let [<Global>] NUKE_DAMAGE: NUKE_DAMAGEType = jsNative
    let [<Global>] REACTIONS: REACTIONSType = jsNative
    let [<Global>] BOOSTS: BOOSTSType = jsNative
    let [<Global>] LOOK_CREEPS: string = jsNative
    let [<Global>] LOOK_ENERGY: string = jsNative
    let [<Global>] LOOK_RESOURCES: string = jsNative
    let [<Global>] LOOK_SOURCES: string = jsNative
    let [<Global>] LOOK_MINERALS: string = jsNative
    let [<Global>] LOOK_STRUCTURES: string = jsNative
    let [<Global>] LOOK_FLAGS: string = jsNative
    let [<Global>] LOOK_CONSTRUCTION_SITES: string = jsNative
    let [<Global>] LOOK_NUKES: string = jsNative
    let [<Global>] LOOK_TERRAIN: string = jsNative
    let [<Global>] Memory: Memory = jsNative
    let [<Global>] Game: Game = jsNative
    let [<Global>] PathFinder: PathFinder = jsNative