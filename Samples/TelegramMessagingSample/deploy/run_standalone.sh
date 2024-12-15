sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0 dotnet-runtime-8.0 aspnetcore-runtime-8.0


rm -rf botticelli/
git clone https://github.com/devgopher/botticelli.git
pushd botticelli/
git checkout release/0.6
git pull

pushd TelegramMessagingSample

dotnet run TelegramMessagingSample.csproj &

echo BOT ID:
cat Data/botId

popd