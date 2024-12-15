function check_and_setup()
{
  read -p "Enter $1 $2: " $1
}

sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0 dotnet-runtime-8.0 aspnetcore-runtime-8.0

check_and_setup http_port "(example:80)"
check_and_setup https_port "(example:8080)"

export ASPNETCORE_ENVIRONMENT=Release   
export ASPNETCORE_URLS="https://0.0.0.0:$https_port;http://0.0.0.0:$http_port"
export SecureStorageConnectionString="User ID=postgres;Password=12345678;Host=127.0.0.1;Port=5432;Database=botticelli_analytics;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;"

rm -rf botticelli/
git clone https://github.com/devgopher/botticelli.git
pushd botticelli/
git checkout release/0.5
git pull

pushd Botticelli.Server.Analytics

nohup dotnet run Botticelli.Server.Analytics.csproj >/dev/null 2>&1

popd