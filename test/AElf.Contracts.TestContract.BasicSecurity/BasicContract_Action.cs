using AElf.Contracts.TestContract.BasicFunction;
using AElf.Sdk.CSharp;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.TestContract.BasicSecurity
{
    public partial class BasicSecurityContract : BasicSecurityContractContainer.BasicSecurityContractBase
    {
        public override Empty InitialBasicSecurityContract(Address input)
        {
            Assert(!State.Initialized.Value, "Already initialized."); 
            
            //set basic1 contract reference
            Assert(input != null, "Basic1Contract address is not exist.");
            State.BasicFunctionContract.Value = input;
            
            State.Initialized.Value = true;
            State.BoolInfo.Value = true;
            State.Int32Info.Value = 0;
            State.UInt32Info.Value = 0;
            State.Int64Info.Value = 0;
            State.UInt64Info.Value = 0;
            State.StringInfo.Value = string.Empty;
            State.BytesInfo.Value = new byte[]{};
            
            return new Empty();
        }
        
        public override Empty TestBoolState(BoolInput input)
        {
            State.BoolInfo.Value = input.BoolValue;
            
            return new Empty();
        }

        public override Empty TestInt32State(Int32Input input)
        {
            var hashCode = input.GetHashCode();
            Context.LogDebug(()=>$"##Int32 HashCode: {hashCode}");
            State.Int32Info.Value = State.Int32Info.Value.Add(hashCode); //test whether state is the same
            
            return new Empty();
        }

        public override Empty TestUInt32State(UInt32Input input)
        {
            State.UInt32Info.Value = State.UInt32Info.Value.Add(input.UInt32Value);
            
            return new Empty();
        }

        public override Empty TestInt64State(Int64Input input)
        {
            State.Int64Info.Value = State.Int64Info.Value.Add(input.Int64Value);
            
            return new Empty();
        }

        public override Empty TestUInt64State(UInt64Input input)
        {
            State.UInt64Info.Value = State.UInt64Info.Value.Add(input.UInt64Value);
            
            return new Empty();
        }

        public override Empty TestStringState(StringInput input)
        {
            if(string.IsNullOrEmpty(State.StringInfo.Value))
                State.StringInfo.Value = string.Empty;

            var hashCodeInfo = input.GetHashCode().ToString();
            Context.LogDebug(()=>$"##String HashCode: {hashCodeInfo}");
            State.StringInfo.Value = State.StringInfo.Value.Append(hashCodeInfo);
            
            return new Empty();
        }

        public override Empty TestBytesState(BytesInput input)
        {
            State.BytesInfo.Value = input.BytesValue.ToByteArray();
            
            return new Empty();
        }

        public override Empty TestProtobufState(ProtobufInput input)
        {
            State.ProtoInfo2.Value = input.ProtobufValue;
            
            return new Empty();
        }

        public override Empty TestComplex1State(Complex1Input input)
        {
            State.BoolInfo.Value = input.BoolValue;
            State.Int32Info.Value = input.Int32Value;
            
            return new Empty();
        }

        public override Empty TestComplex2State(Complex2Input input)
        {
            State.BoolInfo.Value = input.BoolData.BoolValue;
            State.Int32Info.Value = input.Int32Data.Int32Value;
            
            return new Empty();
        }

        public override Empty TestMapped1State(ProtobufInput input)
        {
            var protobufMessage = State.Complex3Info[input.ProtobufValue.Int64Value][input.ProtobufValue.StringValue];
            if(protobufMessage == null)
            {    State.Complex3Info[input.ProtobufValue.Int64Value][input.ProtobufValue.StringValue] = new ProtobufMessage()
                {
                    BoolValue = true,
                    Int64Value = input.ProtobufValue.Int64Value,
                    StringValue = input.ProtobufValue.StringValue
                };
            }
            else
            {
                State.Complex3Info[input.ProtobufValue.Int64Value][input.ProtobufValue.StringValue] =
                    new ProtobufMessage
                    {
                        BoolValue = true,
                        Int64Value = State.Complex3Info[input.ProtobufValue.Int64Value][input.ProtobufValue.StringValue].Int64Value
                            .Add(input.ProtobufValue.Int64Value),
                        StringValue = input.ProtobufValue.StringValue
                    };
            }

            return new Empty();
        }

        public override Empty TestMapped2State(Complex3Input input)
        {
            var tradeMessage = State.Complex4Info[input.From][input.PairA][input.To][input.PairB];
            if (tradeMessage == null)
            {
                input.TradeDetails.Timestamp = Context.CurrentBlockTime;
                State.Complex4Info[input.From][input.PairA][input.To][input.PairB] = input.TradeDetails;
            }
            else
            {
                tradeMessage.FromAmount = tradeMessage.FromAmount.Add(input.TradeDetails.FromAmount);
                tradeMessage.ToAmount = tradeMessage.ToAmount.Add(input.TradeDetails.ToAmount);
                tradeMessage.Timestamp = Context.CurrentBlockTime;

                State.Complex4Info[input.From][input.PairA][input.To][input.PairB] = tradeMessage;
            }
            
            return new Empty();
        }

        //Reference call action
        public override Empty TestExecuteExternalMethod(Int64Input input)
        {
            var feeValue = input.Int64Value.Mul(5).Div(100);
            var betValue = input.Int64Value.Sub(feeValue);
            
            State.Int64Info.Value.Add(feeValue);
            State.BasicFunctionContract.UserPlayBet.Send(new BetInput
            {
                Int64Value = betValue
            });
            
            return new Empty();
        }

        public override Empty TestOriginAddress(Address address)
        {
            State.BasicFunctionContract.ValidateOrigin.Send(address);
            return new Empty();
        }
    }
}