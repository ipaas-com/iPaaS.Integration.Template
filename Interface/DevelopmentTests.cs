using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Integration.Data.Interface
{
    public class DevelopmentTests
    {
        // Example development test running Connection Validation
        public static async Task Integration_Wrapper_ValidateConnection(Integration.Abstract.Connection connection)
        {
            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;
            var response = await wrapper.ValidateConnection();

            // Test with successful and invalid credentials.
            // Are you handling Error responses correctly?
        }

        // Example development test running from your DataModel events
        public static async Task Integration_Template_FromiPaaS_Create(Integration.Abstract.Connection connection)
        {
            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;

            DataModels.Template DevTest1 = new DataModels.Template();
            DevTest1.Id = 19;
            // set other properties for DevTest1
            // set your debug breakpoints in here and step through after executing your DevelopmentTest

            var response = await DevTest1.Create(wrapper);
            
            // Check your response status.  Did everything go OK?
        }

        /*
         * 
        // Example Development Test Structure for Certification
        // Name should be the IntegrationName_MappingCollectionType_Direction_EventType
        public static async Task MeetHue_Transaction_FromiPaaS_Update(Integration.Abstract.Connection connection)
        {
            // Below is a Test Scenario to turn on a Phillips Hue Light named "Mood Light"
            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;

            // Development Tests should start with datamodels that were registered in Interface.Metadata
            // Each field in the subscription mappings for this integration should be represented below setting a value to the corresponding property.
            DataModels.HueAction TestLightAction = new DataModels.HueAction();

            // Highlights of the test
            // 1) Demonstrate how a Built-In Conversion function can be applied in the mappings

            TestLightAction.EventType = DataModels.EventType.Light;
            TestLightAction.SearchName = "Mood Light";
            TestLightAction.LightState = ConversionFunctions.FlashRed();

            await TestLightAction.Update(wrapper);

            // In Comments: Describe the expected outcome and how it can be evidenced
            // The light on my Hue Account named "Mood Light" will have a changed lightstate. 
            // This can be evidenced by camera or by accessing the MeetHue API using the subscription credentials directly.
            // To access it directly, we will make a call to https://api.meethue.com/bridge/{{bridge}}/lights
            // and verify the response.  The response should look like the following.  "Mood Light" is "3" and state.on will equal true afterwards.

            //"3": {
            //    "state": {
            //        "on": true,
            //        "bri": 254,
            //        "hue": 63252,
            //        "sat": 249,
            //        "effect": "none",
            //        "xy": [
            //            0.6616,
            //            0.2828
            //                ],
            //        "alert": "lselect",
            //        "colormode": "hs",
            //        "mode": "homeautomation",
            //        "reachable": true
            //            },
            //    "swupdate": {
            //                "state": "noupdates",
            //        "lastinstall": "2021-10-22T18:36:44"
            //    },
            //    "type": "Color light",
            //    "name": "Mood Light",
            //    "modelid": "LLC011",
            //    "manufacturername": "Signify Netherlands B.V.",
            //    "productname": "Hue bloom",
            //    "capabilities": {
            //                "certified": true,
            //        "control": {
            //                    "mindimlevel": 10000,
            //            "maxlumen": 120,
            //            "colorgamuttype": "A",
            //            "colorgamut": [
            //                [
            //                    0.704,
            //                    0.296
            //                ],
            //                [
            //                    0.2151,
            //                    0.7106
            //                ],
            //                [
            //                    0.138,
            //                    0.08
            //                ]
            //            ]
            //        },
            //        "streaming": {
            //                    "renderer": true,
            //            "proxy": false
            //        }
            //            },
            //    "config": {
            //                "archetype": "huebloom",
            //        "function": "decorative",
            //        "direction": "upwards",
            //        "startup": {
            //                    "mode": "custom",
            //            "configured": true,
            //            "customsettings": {
            //                        "bri": 254,
            //                "xy": [
            //                    0.202,
            //                    0.5619
            //                ]
            //            }
            //                }
            //            },
            //    "uniqueid": "00:17:88:01:00:1c:76:4a-0b",
            //    "swversion": "67.91.1"
            //}

        }
        */

    }

}

