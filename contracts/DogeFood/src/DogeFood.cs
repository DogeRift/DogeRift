using System;
using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace DogeFood
{
    [DisplayName("DogeFood")]
    [ManifestExtra("Author", "DogeRift")]
    [ManifestExtra("Email", "support@DogeRift.com")]
    [ManifestExtra("Description", "DogeRift.com")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", new[] {"*"})]
    public class DogeFood : Nep17Token
    {
        private static StorageMap ContractMetadata => new StorageMap(Storage.CurrentContext, "Metadata");
        private static Transaction Tx => (Transaction) Runtime.ScriptContainer;

        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (!update)
            {
                ContractMetadata.Put("Owner", (ByteString) Tx.Sender);
                Mint(Tx.Sender, BigInteger.Parse("10000000000000000000000"));
            }
        }

        public static void GenerateFood(BigInteger amount)
        {
            VerifyOwner();
            Mint(Tx.Sender, amount);
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            VerifyOwner();
            ContractManagement.Update(nefFile, manifest, null);
        }

        private static void VerifyOwner()
        {
            ByteString owner = ContractMetadata.Get("Owner");
            if (!Runtime.CheckWitness((Neo.UInt160)owner))
            {
                throw new Exception("Only the contract owner can do this");
            }
        }

        public override byte Decimals() => 8;
        public override string Symbol() => "DOGEF";

        public static void Burn(BigInteger amount)
        {
            if (amount <= 0) return;
            if (!Runtime.CheckWitness(Tx.Sender)) return;
            if (BalanceOf(Tx.Sender) < amount) return;
            Nep17Token.Burn(Tx.Sender, amount);
        }
    }
}
