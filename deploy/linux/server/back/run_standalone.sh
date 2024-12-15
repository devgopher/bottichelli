function check_and_setup()
{
  read -p "Enter $1 $2: " $1
}

sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0 dotnet-runtime-8.0 aspnetcore-runtime-8.0

rm -rf botticelli``/
git clone https://github.com/devgopher/botticelli.git
pushd botticelli/
git checkout release/0.6
git pull

pushd Botticelli

check_and_setup http_port "(example:80)"
check_and_setup https_port "(example:8080)"
check_and_setup db_password "(example:12345678)"
check_and_setup email "(example:foo@bar.com)"
check_and_setup email_smtp_server "(example:smtp.bar.com)"
check_and_setup email_smtp_port "(example:465)"
check_and_setup email_smtp_pwd "(smtp server/application password)"
check_and_setup email_use_ssl "(true/false)"
check_and_setup requires_authentification "(true/false)"
check_and_setup bot_info_db "(example: /data/)"

export ASPNETCORE_ENVIRONMENT=Release
export ASPNETCORE_URLS="https://0.0.0.0:$https_port;http://0.0.0.0:$http_port"

export SecureStorageSettings__ConnectionString="Filename=database.db;Connection=shared;Password=$db_password"
export ServerSettings__TokenLifetimeMin=1000
export ServerSettings__SmtpClientOptions__Server=$email_smtp_server
export ServerSettings__SmtpClientOptions__Port=$email_smtp_port
export ServerSettings__SmtpClientOptions__User=$email
export ServerSettings__SmtpClientOptions__Password=$email_smtp_pwd
export ServerSettings__SmtpClientOptions__UseSsl=$email_use_ssl
export ServerSettings__SmtpClientOptions__RequiresAuthentication=$requires_authentification
export ServerSettings__SmtpClientOptions__PreferredEncoding=null
export ServerSettings__SmtpClientOptions__UsePickupDirectory=false
export ServerSettings__SmtpClientOptions__MailPickupDirectory=C:\\Temp
export ServerSettings__SmtpClientOptions__SocketOptions=2
export ServerSettings__ServerEmail=$email
export ServerSettings__ServerUrl=$email_smtp_server
export ServerSettings__BotInfoDb=$bot_info_db

nohup dotnet run Botticelli.Server.csproj & >/dev/null 2>&1

popd