files/ 
contains:
- decryptionkey.bin - this is the AES key to be used for decrypting the test.txt.enc file
- test.txt.enc - this is the envelopeCms file which has created by a public key, the internal encrypted AES  key is already decrypted and available in the decryptionkey.bin file.

source/
contains:
The dotnet8 console application which tries to manually decrypt the encrypted data.

To decrypt the message, we need:
- The decrypted AES 256 key (present in decryptionkey.bin)
- IV (initialization vector)

I believe the IV is the first 16 bytes of the actual encrypted content. 

