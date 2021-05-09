# Удаляем правила
/sbin/iptables -F
/sbin/iptables -X
/sbin/iptables -t nat -F
/sbin/iptables -t nat -X
/sbin/iptables -t mangle -F
/sbin/iptables -t mangle -X
/sbin/iptables -P INPUT ACCEPT
/sbin/iptables -P FORWARD ACCEPT
/sbin/iptables -P OUTPUT ACCEPT

# Разрешить локалный траффик
/sbin/iptables -A INPUT -i lo -j ACCEPT

# Белый список IP
/sbin/iptables -A INPUT -s 89.149.200.99 -j ACCEPT  #local
/sbin/iptables -A INPUT -s 127.0.0.1 -j ACCEPT

# Разрешить сессии
/sbin/iptables -A INPUT -m conntrack --ctstate RELATED,ESTABLISHED -j ACCEPT

# Блокируем порты
/sbin/iptables -A INPUT -p tcp --dport 1000 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1000 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1001 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1001 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1002 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1002 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1003 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1003 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1004 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1004 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1005 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1005 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1006 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1006 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1007 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1007 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1008 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1008 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1009 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1009 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1010 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1010 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1011 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1011 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1012 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1012 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1013 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1013 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1014 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1014 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1015 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1015 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1016 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1016 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1017 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1017 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1018 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1018 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1019 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1019 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1020 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1020 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1021 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1021 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1022 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1022 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1023 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1023 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1024 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1024 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1025 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1025 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1026 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1026 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1027 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1027 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1028 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1028 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1029 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1029 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1030 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1030 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1031 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1031 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1032 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1032 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1033 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1033 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1034 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1034 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1035 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1035 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1036 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1036 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1037 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1037 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1038 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1038 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1039 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1039 -j DROP
/sbin/iptables -A INPUT -p tcp --dport 1040 -j DROP
/sbin/iptables -A INPUT -p udp --dport 1040 -j DROP

iptables -L -n -v
#iptables -n -L -v --line-numbers
#iptables -D INPUT line
