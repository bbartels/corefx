﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        private static void GetRuntimeClassInfo(object value, ref JsonClassInfo jsonClassInfo, JsonSerializerOptions options)
        {
            if (value != null)
            {
                Type runtimeType = value.GetType();

                // Nothing to do for typeof(object)
                if (runtimeType != typeof(object))
                {
                    jsonClassInfo = options.GetOrAddClass(runtimeType);
                }
            }
        }

        private static void GetRuntimePropertyInfo(object value, JsonClassInfo jsonClassInfo, ref JsonPropertyInfo jsonPropertyInfo, JsonSerializerOptions options)
        {
            if (value != null)
            {
                Type runtimeType = value.GetType();

                // Nothing to do for typeof(object)
                if (runtimeType != typeof(object))
                {
                    jsonPropertyInfo = jsonClassInfo.CreatePolymorphicProperty(jsonPropertyInfo, runtimeType, options);
                }
            }
        }

        private static void VerifyValueAndType(object value, Type type)
        {
            if (type == null)
            {
                if (value != null)
                {
                    throw new ArgumentNullException(nameof(type));
                }
            }
            else if (value != null)
            {
                if (!type.IsAssignableFrom(value.GetType()))
                {
                    ThrowHelper.ThrowArgumentException_DeserializeWrongType(type, value);
                }
            }
        }

        private static void WriteNull(
            ref JsonWriterState writerState,
            IBufferWriter<byte> bufferWriter)
        {
            Utf8JsonWriter writer = new Utf8JsonWriter(bufferWriter, writerState);
            writer.WriteNullValue();
            writer.Flush(true);
        }

        private static byte[] WriteCoreBytes(object value, Type type, JsonSerializerOptions options)
        {
            options ??= JsonSerializerOptions.s_defaultOptions;

            byte[] result;

            using (var output = new ArrayBufferWriter<byte>(options.DefaultBufferSize))
            {
                WriteCore(output, value, type, options);
                result = output.WrittenMemory.ToArray();
            }

            return result;
        }

        private static string WriteCoreString(object value, Type type, JsonSerializerOptions options)
        {
            options ??= JsonSerializerOptions.s_defaultOptions;
            string result;

            using (var output = new ArrayBufferWriter<byte>(options.DefaultBufferSize))
            {
                WriteCore(output, value, type, options);
                result = JsonReaderHelper.TranscodeHelper(output.WrittenMemory.Span);
            }

            return result;
        }

        private static void WriteCore(ArrayBufferWriter<byte> output, object value, Type type, JsonSerializerOptions options)
        {
            Debug.Assert(type != null || value == null);

            var writerState = new JsonWriterState(options.GetWriterOptions());
            var writer = new Utf8JsonWriter(output, writerState);

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                //  We treat typeof(object) special and allow polymorphic behavior.
                if (type == typeof(object))
                {
                    type = value.GetType();
                }

                WriteStack state = default;
                state.Current.Initialize(type, options);
                state.Current.CurrentValue = value;

                Write(ref writer, -1, options, ref state);
            }

            writer.Flush(isFinalBlock: true);
        }
    }
}
