using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BibliotecasAPI.BLL.Services.Interfaces;
using System.Data.Common;

namespace BibliotecasAPI.BLL.Services.Impl
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        private readonly string connectionString;

        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {
            //connectionString  = configuration.GetConnectionString("AzureStorageConnection")!;
            connectionString  = configuration.GetValue<string>("AzureStorageConnection")!;
        }
        public async Task<string> Almacenar(string? contenedor, IFormFile archivo)
        {
            BlobContainerClient cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            cliente.SetAccessPolicy(PublicAccessType.Blob);

            string extension = Path.GetExtension(archivo.FileName);
            string nombreArchivo = $"{Guid.NewGuid()}{extension}";
            BlobClient blob = cliente.GetBlobClient(nombreArchivo);
            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = archivo.ContentType;
            await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);
            return blob.Uri.ToString();
        }

        public async Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }
            BlobContainerClient cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();

            string nombreArchivo = Path.GetFileName(ruta);
            BlobClient blob = cliente.GetBlobClient(nombreArchivo);
            await blob.DeleteIfExistsAsync();

        }
    }
}
