function check_and_setup()
{
  read -p "Enter $1 $2: " $1
}

check_and_setup db_password "(example:12345678)"
check_and_setup email_addr "(example:foo@bar.com)"
check_and_setup email_smtp_server "(example:smtp.bar.com)"
check_and_setup email_smtp_port "(example:465)"
check_and_setup email_smtp_pwd "(smtp server/application password)"
check_and_setup email_use_ssl "(true/false)"
check_and_setup requires_authentification "(true/false)"
check_and_setup bot_info_db "(example: /data/botInfo.db)"

export SecureStorageSettings__ConnectionString="Filename=/data/database.db;Connection=shared;Password=$db_password"
export ServerSettings__TokenLifetimeMin=1000
export ServerSettings__SmtpClientOptions__Server=$email_smtp_server
export ServerSettings__SmtpClientOptions__Port=$email_smtp_port
export ServerSettings__SmtpClientOptions__User=$email_addr
export ServerSettings__SmtpClientOptions__ServerEmail=$email_addr
export ServerSettings__SmtpClientOptions__Password=$email_smtp_pwd
export ServerSettings__SmtpClientOptions__UseSsl=$email_use_ssl
export ServerSettings__SmtpClientOptions__RequiresAuthentication=$requires_authentification
export ServerSettings__SmtpClientOptions__PreferredEncoding=null
export ServerSettings__SmtpClientOptions__UsePickupDirectory=false
export ServerSettings__SmtpClientOptions__MailPickupDirectory=/tmp
export ServerSettings__SmtpClientOptions__SocketOptions=2
export ServerSettings__ServerEmail=$email
export ServerSettings__ServerUrl=$email_smtp_server
export ServerSettings__BotInfoDb=$bot_info_db

mkdir /data
mkdir /logs
mkdir /tmp

cp database.db Data
dotnet dev-certs https --clean
dotnet dev-certs https -ep /usr/local/share/ca-certificates/botticelli_server_dev_cert.crt -p 12345678 --format PEM
sudo update-ca-certificates

docker build --tag "botticelli_server_back_dev:0.6" . --file dockerfile_admin_back.development
docker run --restart=always \
	-p 7247:7247 \
	-p 5042:5042 \
	-p 465:465 \
	-e ASPNETCORE_ENVIRONMENT="Development" \
	-e SecureStorageSettings__ConnectionString="${SecureStorageSettings__ConnectionString}" \
	-e ServerSettings__TokenLifetimeMin="${ServerSettings__TokenLifetimeMin}" \
	-e ServerSettings__SmtpClientOptions__Server="${ServerSettings__SmtpClientOptions__Server}" \
	-e ServerSettings__SmtpClientOptions__Port="${ServerSettings__SmtpClientOptions__Port}" \
	-e ServerSettings__SmtpClientOptions__User="${ServerSettings__SmtpClientOptions__User}" \
	-e ServerSettings__SmtpClientOptions__Password="${ServerSettings__SmtpClientOptions__Password}" \
	-e ServerSettings__SmtpClientOptions__UseSsl="${ServerSettings__SmtpClientOptions__UseSsl}" \
	-e ServerSettings__SmtpClientOptions__RequiresAuthentication="${ServerSettings__SmtpClientOptions__RequiresAuthentication}" \
	-e ServerSettings__SmtpClientOptions__PreferredEncoding="${ServerSettings__SmtpClientOptions__PreferredEncoding}" \
	-e ServerSettings__SmtpClientOptions__UsePickupDirectory="${ServerSettings__SmtpClientOptions__UsePickupDirectory}" \
	-e ServerSettings__SmtpClientOptions__MailPickupDirectory="${ServerSettings__SmtpClientOptions__MailPickupDirectory}" \
	-e ServerSettings__SmtpClientOptions__SocketOptions="${ServerSettings__SmtpClientOptions__SocketOptions}" \
	-e ServerSettings__ServerEmail="${ServerSettings__ServerEmail}" \
	-e ServerSettings__BotInfoDb="${ServerSettings__BotInfoDb}" \
    	-e ServerSettings__ServerUrl="${ServerSettings__ServerUrl}" \
   	-v /data:/data \
    	-v /logs:/logs \
    	-v /tmp:/tmp \
    	-d  botticelli_server_back_dev:0.6