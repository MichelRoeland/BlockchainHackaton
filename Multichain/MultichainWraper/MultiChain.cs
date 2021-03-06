﻿using Newtonsoft.Json;
using Stoneycreek.libraries.MultichainWrapper.Properties;
using Stoneycreek.libraries.MultichainWrapper.Structs;
using StoneyCreek.Services.Blockchain.DataContracts;
using System;
using System.Configuration;
using System.Linq;
using System.Resources;

namespace Stoneycreek.libraries.MultichainWrapper
{
    public class MultiChain : MultiChainBase
    {
        #region constructor

        public MultiChain(string streamname = null, ProcessWrapper processWrapper = null) : base(streamname, processWrapper)
        {
        }

        #endregion constructor

        #region Public Methods

        public byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null) throw new ArgumentNullException("hexString");
            if (hexString.Length % 2 != 0) throw new ArgumentException("hexString must have an even length", "hexString");
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string currentHex = hexString.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(currentHex, 16);
            }
            return bytes;
        }

        public string GrantPermisions(GrantPermisionsData data)
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + this.ResourceManager.GetString("Grant");
            return FormatAndExecute(data, commandtext);
        }

        public string RevokePermisions(GrantPermisionsData data)
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + this.ResourceManager.GetString("Revoke");
            return FormatAndExecute(data, commandtext);
        }

        public Permissions ListPermissions()
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + this.ResourceManager.GetString("Listpermissions");
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);
                //.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("[", string.Empty).Replace("[", string.Empty).Replace("{", string.Empty);

            result = "{\"permissions\" : " + result + "}";
            var tmp = JsonConvert.DeserializeObject<Permissions>(result);
            return tmp;
        }

        public Streams ListStreams()
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + this.ResourceManager.GetString("Liststreams");
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            result = "{\"streams\" : " + result + "}";
            var tmp = JsonConvert.DeserializeObject<Streams>(result);
            return tmp;
        }

        public string CreateNewAddress()
        {
            var command = ChainLocation + ClientName + " " + Chainname + " " + this.ResourceManager.GetString("Getnewaddress");
            var info = this.CommandExecuter.ExecuteCommand(command, "");
            return info.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public string CreateNewStream(bool isOpenStream, string streamname)
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " "
                              + string.Format(this.ResourceManager.GetString("CreateStream"), "stream", streamname, isOpenStream ? "true" : "false");
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            return result;
        }

        public string PublishMessage(PublishMessageData data)
        {
            // multichain - cli mytestchain publish test "hello world" 48656C6C6F20576F726C64210A
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + string.Format(this.ResourceManager.GetString("Publish"), data.StreamName, "\"" + data.Key + "\"", data.HexString);
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            return result.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public string PublishMessageFrom(PublishMessageData data)
        {
            // multichain - cli mytestchain publish test "hello world" 48656C6C6F20576F726C64210A
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + string.Format(this.ResourceManager.GetString("Publishfrom"), data.FromAddress, data.StreamName, "\"" + data.Key + "\"", data.HexString);
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            return result.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }
        
        public string Subscribe(string streamname)
        {
            // multichain - cli mytestchain publish test "hello world" 48656C6C6F20576F726C64210A
            var commandtext = ChainLocation + ClientName + " " + Chainname + " "
                              + string.Format(this.ResourceManager.GetString("Subscribe"), streamname, "true");
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            return result.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public string UnSubscribe(string streamName)
        {
            // multichain - cli mytestchain publish test "hello world" 48656C6C6F20576F726C64210A
            var commandtext = ChainLocation + ClientName + " " + Chainname + " "
                              + string.Format(this.ResourceManager.GetString("Unsubscribe"), streamName);
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            return result.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public StreamKey GetStreamKeys(string streamname)
        {
            // multichain - cli mytestchain publish test "hello world" 48656C6C6F20576F726C64210A
            var commandtext = ChainLocation + ClientName + " " + Chainname + " "
                              + string.Format(this.ResourceManager.GetString("Liststreamkeys"), streamname);
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            result = "{\"streamkeys\" : " + result + "}";
            var tmp = JsonConvert.DeserializeObject<StreamKey>(result);
            return tmp;
        }

        //Multichain-cli.exe mytestchain liststreamkeys pinjo
        public StreamItem GetStreamItem(string streamname, string transactionId)
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " "
                              + string.Format(this.ResourceManager.GetString("Getstreamitem"), streamname, transactionId);
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);
            var tmp = JsonConvert.DeserializeObject<StreamItem>(result);
            return tmp;
        }

        public StreamItems GetStreamItemByKey(string streamname, string key)
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + string.Format(this.ResourceManager.GetString("Liststreamkeyitems"), streamname, key);
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            if (result.Any())
            {
                result = "{\"streamitems\" : " + result + "}";
            }


            return JsonConvert.DeserializeObject<StreamItems>(result);
        }

        public string SignMessage(string privatekey, string message)
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + string.Format(this.ResourceManager.GetString("Signmessage"), privatekey, "\"" + message + "\"");
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            return result.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public string VerifyMessage(MessageData data)
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + string.Format(this.ResourceManager.GetString("Verifymessage"), data.address, data.signature, "\"" + data.message + "\"");
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            return result.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public string ImportPrivateKey(string privatekey)
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " "
                              + string.Format(this.ResourceManager.GetString("Importprivkey"), privatekey);
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            return result.Replace("\r", string.Empty).Replace("\n", string.Empty);
            ;
        }

        public KeyPairs CreateKeys()
        {
            var commandtext = ChainLocation + ClientName + " " + Chainname + " " + string.Format(this.ResourceManager.GetString("Createkeypairs"));
            var result = this.CommandExecuter.ExecuteCommand(commandtext, null);

            result = "{\"keypairs\" : " + result + "}";
            var tmp = JsonConvert.DeserializeObject<KeyPairs>(result);

            return tmp;
        }

        public string GetValidStringdata(string data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            return "\"" + data + "\"";
        }

        #endregion Public Methods

        #region Private Methods

        private string FormatAndExecute(GrantPermisionsData data, string commandtext)
        {
            var permissionstring = string.Join(",", data.Permissions.Select(f => f.ToString()).ToArray());

            var command = string.Format(
                    commandtext,
                    data.Address,
                    permissionstring,
                    string.Empty,
                    string.Empty,
                    data.NativeAmount,
                    this.GetValidStringdata(data.Comment),
                    this.GetValidStringdata(data.CommentTo)).Trim();

            return this.CommandExecuter.ExecuteCommand(command, null).Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        #endregion Private Methods
    }
}
