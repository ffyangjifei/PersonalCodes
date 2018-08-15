import urllib.request, urllib.parse, urllib.error
import http.cookiejar
import sys
import io
from urllib import request
import re
import json
import time
# from PIL import Image

sys.stdout = io.TextIOWrapper(sys.stdout.buffer,encoding='utf8') #改变标准输出的默认编码
user_agent = r'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36'
cookie=r'pgv_pvi=8425347072; pgv_pvid=2579887115; pt2gguin=o1021967981; uin=o1021967981; skey=@ovAXpGwH3; ptisp=ctc; RK=DQTdIg9wbX; ptcz=0df2a744c5bf8dd74d497aabac676f7e6ec422e430baf02097562ad004becdaf; p_uin=o1021967981; pt4_token=Spc9dxUYDuVHUxuRSRj8sdzMj67mZhPmP6KGOF2crPY_; p_skey=xxs2Hh6eyB9xhMR3-kxWDxq0LMSg-KoZQVQgmQktmdg_; Loading=Yes; qz_screen=1920x1080; qqmusic_uin=; qqmusic_key=; qqmusic_fromtag=; pgv_info=ssid=s2074052178; QZ_FE_WEBP_SUPPORT=1; __Q_w_s_hat_seed=1; cpu_performance_v8=1; qzmusicplayer=qzone_player_1021967981_1534339930415; v6uin=1021967981|qzone_player';
headers = {'User-Agent': user_agent, 'Connection': 'keep-alive','Cookie': cookie}

num=20
for index in range(0,100):
    start = index * 20
    get_url = 'https://user.qzone.qq.com/proxy/domain/m.qzone.qq.com/cgi-bin/new/get_msgb?uin=1021967981&hostUin=1021967981&start='+str(start)+'&s=0.7495392448150253&format=jsonp&num='+str(num)+'&inCharset=utf-8&outCharset=utf-8&g_tk=1918311862&qzonetoken=abd385e644fdb1155581ba7f402c72835d6155e9647ab280bf7873fbb00f4a24fa892261fe8344a4f91e&g_tk=1918311862'

    req = request.Request(get_url)
    req.add_header('cookie', cookie)
    req.add_header('User-Agent', 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36')

    html = urllib.request.urlopen(req)
    content= html.read().decode('utf-8')
    cstr=content[10:-2]
    data= json.loads(cstr)
    for d in data['data']['commentList']:
        if (d['uin']==396160598):
            # if(str(d['ubbContent']).__contains__('img')):
            #     path =d['ubbContent'][13,-6]
            #     print(d['pubtime'] + '##' + path)
            #     # im = Image.open(path)
            #     # im.show()
            # else:
                print(d['pubtime'] + '##' + d['ubbContent'])
    # time.sleep(0.5)

# print('You have not solved this problem' in get_response.read().decode())


# GET https://user.qzone.qq.com/proxy/domain/m.qzone.qq.com/cgi-bin/new/get_msgb?uin=1021967981&hostUin=1021967981&start=0&s=0.7495392448150253&format=jsonp&num=10&inCharset=utf-8&outCharset=utf-8&g_tk=1918311862&qzonetoken=abd385e644fdb1155581ba7f402c72835d6155e9647ab280bf7873fbb00f4a24fa892261fe8344a4f91e&g_tk=1918311862 HTTP/1.1
# Host: user.qzone.qq.com
# Connection: keep-alive
# User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36
# Accept: */*
# Referer: https://user.qzone.qq.com/1021967981/334
# Accept-Encoding: gzip, deflate, br
# Accept-Language: zh-CN,zh;q=0.9
# Cookie: pgv_pvi=8425347072; pgv_pvid=2579887115; pt2gguin=o1021967981; uin=o1021967981; skey=@ovAXpGwH3; ptisp=ctc; RK=DQTdIg9wbX; ptcz=0df2a744c5bf8dd74d497aabac676f7e6ec422e430baf02097562ad004becdaf; p_uin=o1021967981; pt4_token=Spc9dxUYDuVHUxuRSRj8sdzMj67mZhPmP6KGOF2crPY_; p_skey=xxs2Hh6eyB9xhMR3-kxWDxq0LMSg-KoZQVQgmQktmdg_; Loading=Yes; qz_screen=1920x1080; qqmusic_uin=; qqmusic_key=; qqmusic_fromtag=; pgv_info=ssid=s2074052178; QZ_FE_WEBP_SUPPORT=1; __Q_w_s_hat_seed=1; cpu_performance_v8=1; qzmusicplayer=qzone_player_1021967981_1534339930415; v6uin=1021967981|qzone_player
#
#
# HTTP/1.1 200 OK
# Date: Wed, 15 Aug 2018 13:32:50 GMT
# Content-Type: application/x-javascript; charset=utf-8
# Content-Length: 938
# Connection: keep-alive
# X-Powered-By: TSW/Node.js
# Server: QZHTTP-2.38.18
# Cache-Control: max-age=3600
# Vary: Origin, Accept
# Mod-Map: proxy_domain:photo.v7/module/proxy/sync.js
# Content-Encoding: gzip
# x-stgw-ssl-info: 46f455f72d08c9182c6130ee85a4799b|0.149|1534339970.517|1|r|I|TLSv1.2|ECDHE-RSA-AES128-GCM-SHA256|12000|0
