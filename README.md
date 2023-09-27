# DynamicMachine

Bu kod şu anda basit konsol çağrılarını işleyebiliyor.

- Programın amacı runtime işlenen mekanizma ile orjinal kodu korumaktır.
- İlk önce korunacak (.exe) veya (.dll) dosyasının hedef yöntemi alınır. (Henüz otomatik değil)
- Yöntem içindeki tüm instruction'lar aynı yönteme çalışırken oluşacak şekilde eklenir.

## Geliştiriliyor;

- Geliştirilmesi tamamlandığında mükemmel bir koruma sağlayacaktır.
- DynamicMachine ile korunan programlar dumplanamaz / cracklenmesi zordur.

## Hatırlatma

bin\debug içindeki dnlib kütüphanesini başvurulara eklemeyi unutmayın.


# güncellemeler

**V1.3**
- Artık mono ve dnlib ile obfuscation daha gelişmiş bir şekilde yapılıyor.
- DynamicMachine, kendi temel korumasını yapmasının ardından bazı koruma yöntemleri GalaxyProtector'den alındı, bir klasör içinde hepsini bulabilirsiniz. Desync3'a teşekkürler.

**V1.2**
- Geçici basit düzeltmeler.

**V1.1**
- En büyük geliştirmelerden biri, artık DynamicMachine bir class olarak hedef programın içine inject oluyor. (korunacak programın içine / ..ctor )
- Yeni OpCode destekleri.

**V1.0**
- İlk kod yayınlandı.
