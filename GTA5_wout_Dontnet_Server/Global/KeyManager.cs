﻿using GTANetworkServer;
using System;
using System.Linq;
using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Data.Models;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Groups;
using TheGodfatherGM.Server.Property;
using TheGodfatherGM.Server.Vehicles;

namespace TheGodfatherGM.Server.Global
{
    class KeyManager : Script
    {    
        public KeyManager()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }        
        private void OnClientEventTrigger(Client player, string eventName, object[] args)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            var character = characterController.Character;
            var formatName = character.Name.Replace("_", " ");

            if (eventName == "onKeyDown")
            {
                if ((int)args[0] == 2)
                {
                    PropertyController propertyController = player.getData("AT_PROPERTY");
                    if ((propertyController = player.getData("AT_PROPERTY")) != null)
                    {
                        propertyController.PropertyDoor(player);
                    }
                }
                else if((int)args[0] == 3)
                {
                    if(player.isInVehicle)
                    {
                        player.vehicle.specialLight = true;
                    }
                }
                else if ((int)args[0] == 4)
                {
                    if (player.isInVehicle)
                    {
                        player.vehicle.specialLight = true;
                    }
                }
                else if ((int)args[0] == 5)
                {
                    if (player.isInVehicle) VehicleController.TriggerDoor(player.vehicle, 4);
                }
                else if ((int)args[0] == 6)
                {
                    if (player.isInVehicle) VehicleController.TriggerDoor(player.vehicle, 5);                    
                }
                else if ((int)args[0] == 8)
                {
                    CharacterController.StopAnimation(player);
                }
                // Get info about Player:
                else if ((int)args[0] == 9)
                {                    
                    if (character == null) return;
                    var job = "";
                    switch (character.JobId)
                    {
                        case JobsIdNonDataBase.Homeless: job = "Бомж"; break;
                        case JobsIdNonDataBase.Loader1: job = "Грузчик: С1"; break;
                        case JobsIdNonDataBase.Loader2: job = "Грузчик: С2"; break;
                        case JobsIdNonDataBase.TaxiDriver: job = "Таксист"; break;
                        case JobsIdNonDataBase.BusDriver1: job = "Водитель автобуса: М1"; break;
                        case JobsIdNonDataBase.BusDriver2: job = "Водитель автобуса: М2"; break;
                        case JobsIdNonDataBase.BusDriver3: job = "Водитель автобуса: М3"; break;
                        case JobsIdNonDataBase.BusDriver4: job = "Водитель автобуса: М4"; break;
                    }

                    var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == character.ActiveGroupID);
                    var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                    var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());
                    
                    var driverLicense = character.DriverLicense == 1 ? "Да" : "Нет";                   

                    API.shared.triggerClientEvent(player, "character_menu",
                        2,                                                  // 0
                        "Ваша статистика",                                  // 1
                        "Ваше имя: " + formatName,                          // 2
                        character.Age,                                      // 3
                        character.Level.ToString(),                         // 4
                        job,                                                // 5
                        character.Bank.ToString(),                          // 6
                        driverLicense,                                      // 7
                        character.Debt,                                     // 8
                        EntityManager.GetDisplayName(groupType),            // 9
                        EntityManager.GetDisplayName(groupExtraType),       // 10
                        character.Material,                                 // 11
                        CharacterController.IsCharacterInMafia(character),  // 12
                        CharacterController.IsCharacterInGang(character));  // 13
                }
                // GET info about car
                else if ((int)args[0] == 10)
                {
                    VehicleController vehicleController;
                    
                    bool inVehicleCheck;
                    if (player.isInVehicle)
                    {
                        vehicleController = EntityManager.GetVehicle(player.vehicle);
                        inVehicleCheck = true;
                    }
                    else
                    {
                        vehicleController = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 2.5f);
                        inVehicleCheck = false;
                    }
                    if (vehicleController == null)
                    {
                        API.sendNotificationToPlayer(player, "Вы находитесь далеко от транспорта.");
                        return;
                    }
                    
                    int engineStatus = 0;
                    if (API.getVehicleEngineStatus(vehicleController.Vehicle)) engineStatus = 1;

                    int driverDoorStatus = 1;
                    if (API.getVehicleLocked(vehicleController.Vehicle)) driverDoorStatus = 0;
                    var fuel = vehicleController.VehicleData.FuelTank;

                    var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == character.ActiveGroupID);
                    var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                    string owner;
                    if (vehicleController.VehicleData.GroupId == null) owner = "Владелец: " + formatName;
                    else owner = "Владелец: " + EntityManager.GetDisplayName(groupType);

                    API.shared.triggerClientEvent(player, "vehicle_menu",
                        2,                                          // 0
                        "Меню транспорта",                          // 1
                        owner,                                      // 2
                        engineStatus,                               // 3
                        fuel.ToString(),                            // 4
                        inVehicleCheck,                             // 5
                        driverDoorStatus,                           // 6
                        vehicleController.VehicleData.Material);    // 7    
                        }
                // Get info about work possibilities:
                else if ((int)args[0] == 12)
                {
                    if (character == null) return;
                    var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == character.ActiveGroupID);
                    var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                    var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                    var playerWeapons = API.getPlayerWeapons(player);
                    var weaponList = "";
                    for (var i = 0; i < playerWeapons.Length; i++)
                        weaponList += playerWeapons[i] + "-";

                    var materialProperty = new Data.Property();

                    var moneyStockGroup = new Group();
                    var groupStockName = GroupController.GetGroupStockName(character);                    
                    var moneyBankGroup = character.GroupType * 100;

                    var gangRank = (int)groupExtraType - (int)groupType * 100;

                    try { materialProperty = ContextFactory.Instance.Property.First(x => x.Name == groupStockName); }
                    catch (Exception)
                    {
                        // ignored
                    }
                    try { moneyStockGroup = ContextFactory.Instance.Group.First(x => x.Id == moneyBankGroup); }
                    catch (Exception)
                    {
                        // ignored
                    }

                    var gangsSectors = GroupController.GetGangsSectors();
                    var gangCurrentSector = CharacterController.InWhichSectorOfGhetto(player).Split(';');
                    var gangCurrentSectorData = GroupController.GetGangSectorData(Convert.ToInt32(gangCurrentSector[0]), Convert.ToInt32(gangCurrentSector[1]));
                    if (gangCurrentSectorData > 100) gangCurrentSectorData /= 10;
                    var isSectorInYourGang = gangCurrentSectorData == (int)groupType;
                    if (gangCurrentSectorData > 100 && isSectorInYourGang == false)
                        isSectorInYourGang = true;

                    API.shared.triggerClientEvent(player, "workposs_menu",
                         1,                                                                                  // 0
                         character.ActiveGroupID,                                                            // 1
                         character.JobId,                                                                    // 2
                         character.TempVar,                                                                  // 3
                         character.Admin,                                                                    // 4
                         EntityManager.GetDisplayName(groupType),                                            // 5
                         EntityManager.GetDisplayName(groupExtraType),                                       // 6
                         character.Material,                                                                 // 7
                         CharacterController.IsCharacterInGang(characterController),                         // 8
                         CharacterController.IsCharacterGangBoss(characterController),                       // 9
                         CharacterController.IsCharacterArmyHighOfficer(characterController.Character),      // 10
                         CharacterController.IsCharacterInGhetto(player),                                    // 11
                         CharacterController.IsCharacterArmyGeneral(characterController),                    // 12
                         weaponList,                                                                         // 13
                         character.OID,                                                                      // 14
                         materialProperty.Stock,                                                             // 15
                         moneyStockGroup.MoneyBank,                                                          // 16
                         CharacterController.IsCharacterHighRankInGang(character),                           // 17
                         gangRank,                                                                           // 18
                         (int)groupType,                                                                     // 19
                         gangsSectors,                                                                       // 20
                         isSectorInYourGang,                                                                 // 21
                         groupStockName,                                                                     // 22
                         CharacterController.IsCharacterInMafia(characterController));                       // 23

                }
                else if ((int)args[0] == 13)
                {
                }
                else if ((int)args[0] == 14)
                {
                }
            }
        }
    }
}
