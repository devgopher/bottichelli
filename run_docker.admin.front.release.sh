function check_and_setup()
{
  read -p "Enter $1 $2: " $1
}

check_and_setup http_port "(example:80)"
check_and_setup https_port "(example:8080)"
check_and_setup backend_url "(example: https://10.10.10.10:50000)"
check_and_setup analytics_url "(example: https://10.10.10.10:50055)"

mkdir /logs

dotnet dev-certs https --clean
dotnet dev-certs https -ep /usr/local/share/ca-certificates/botticelli_server_dev_cert.crt -p 12345678 --format pem
sudo update-ca-certificates

docker build --tag "botticelli_server_front_release:0.6" . --file dockerfile_admin_front.release

docker run --restart=always --net=host \
	-p ${https_port}:7247 \
	-p ${http_port}:5042 \
	-e ASPNETCORE_ENVIRONMENT="Release" \
	-e BackSettings__BackUrl="${back_url}" \
	-e BackSettings__AnalyticsUrl="${analytics_url}" \
    -v /logs:/logs \
    -d  botticelli_server_front_release:0.6