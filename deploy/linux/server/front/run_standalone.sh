function check_and_setup()
{
  read -p "Enter $1 $2: " $1
}

sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0 dotnet-runtime-8.0 aspnetcore-runtime-8.0

rm -rf botticelli/
git clone https://github.com/devgopher/botticelli.git
pushd botticelli/
git checkout release/0.6
git pull

pushd Botticelli.Server.FrontNew

check_and_setup http_port "(example:80)"
check_and_setup https_port "(example:8080)"
check_and_setup backend_url "(example: https://10.10.10.10:50000)"
check_and_setup analytics_url "(example: https://10.10.10.10:50055)"

export ASPNETCORE_ENVIRONMENT=Release
export ASPNETCORE_URLS="https://0.0.0.0:$https_port;http://0.0.0.0:$http_port"
export BackSettings__BackUrl=$backend_url/v1/
export BackSettings__AnalyticsUrl=$analytics_url/v1/


nohup dotnet run Botticelli.Server.FrontNew.csproj >/dev/null 2>&1 &

popd