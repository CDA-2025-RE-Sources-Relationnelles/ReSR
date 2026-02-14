using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReSR.Application.Ports;

namespace ReSR.Infrastructure.ValueConverters;
internal class EncryptedConverter(
    IEncryptionService encryptionService,
    ConverterMappingHints? mappingHints = default
) : ValueConverter<string, string>(
    x => encryptionService.Encrypt(x),
    x => encryptionService.Decrypt(x),
    mappingHints
) {}