// See https://aka.ms/new-console-template for more information
using Amazon.S3;
using Amazon.S3.Model;

AmazonS3Client client = new();

var buckets = await client.ListBucketsAsync();

ListBuckets(buckets.Buckets);

var uniqueBucketName = $"{Guid.NewGuid()}";

Console.WriteLine($"Novo bucket: {uniqueBucketName}");

await client.PutBucketAsync(uniqueBucketName);

buckets = await client.ListBucketsAsync();
ListBuckets(buckets.Buckets);

var bucketDeletionTasks = new List<Task>(buckets.Buckets.Count);

foreach (var bucket in buckets.Buckets)
    bucketDeletionTasks.Add(DeleteBucketAsync(bucket));

await Task.WhenAll(bucketDeletionTasks);


buckets = await client.ListBucketsAsync();

ListBuckets(buckets.Buckets);

void ListBuckets(IEnumerable<S3Bucket> buckets){
    Console.WriteLine("Buckets disponíveis: ");
    if (buckets is null)
    {
        Console.WriteLine("Nenhum bucket foi encontrado na conta");
        return;
    }
    
    foreach (var bucket in buckets)
        Console.WriteLine($"{bucket.BucketName} - {bucket.CreationDate}");
}

async Task DeleteBucketAsync(S3Bucket bucket)
{
    try
    {
        var request = new DeleteBucketRequest()
        {
            BucketName = bucket.BucketName,
            BucketRegion = S3Region.USEast1
        };

        await client.DeleteBucketAsync(request);
    }
    catch
    {
        Console.WriteLine($"Não foi possível deletar o bucket {bucket.BucketName}");
    }
}