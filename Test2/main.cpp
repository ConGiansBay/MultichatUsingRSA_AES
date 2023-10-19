#include <iostream>
#include <cstdint>
#include <cstdlib>
#include <ctime>
#include <cstring>
#include <string>
#include <vector>
#include <iomanip>
using namespace std;

const int base = 1000000000; const int base_digits = 9;
struct largeNumber_128 {
    vector<int> a; int sign;

    largeNumber_128() :
        sign(1) {
    }

    largeNumber_128(long long v) {
        *this = v;
    }

    largeNumber_128(const string& s) {
        read(s);
    }

    void operator=(const largeNumber_128& v) {
        sign = v.sign;
        a = v.a;
    }

    void operator=(long long v) {
        sign = 1;
        if (v < 0)
            sign = -1, v = -v;
        for (; v > 0; v = v / base)
            a.push_back(v % base);
    }

    largeNumber_128 operator+(const largeNumber_128& v) const {
        if (sign == v.sign) {
            largeNumber_128 res = v;

            for (int i = 0, carry = 0; i < (int)max(a.size(), v.a.size()) || carry; ++i) {
                if (i == (int)res.a.size())
                    res.a.push_back(0);
                res.a[i] += carry + (i < (int)a.size() ? a[i] : 0);
                carry = res.a[i] >= base;
                if (carry)
                    res.a[i] -= base;
            }
            return res;
        }
        return *this - (-v);
    }

    largeNumber_128 operator-(const largeNumber_128& v) const {
        if (sign == v.sign) {
            if (abs() >= v.abs()) {
                largeNumber_128 res = *this;
                for (int i = 0, carry = 0; i < (int)v.a.size() || carry; ++i) {
                    res.a[i] -= carry + (i < (int)v.a.size() ? v.a[i] : 0);
                    carry = res.a[i] < 0;
                    if (carry)
                        res.a[i] += base;
                }
                res.trim();
                return res;
            }
            return -(v - *this);
        }
        return *this + (-v);
    }

    void operator*=(int v) {
        if (v < 0)
            sign = -sign, v = -v;
        for (int i = 0, carry = 0; i < (int)a.size() || carry; ++i) {
            if (i == (int)a.size())
                a.push_back(0);
            long long cur = a[i] * (long long)v + carry;
            carry = (int)(cur / base);
            a[i] = (int)(cur % base);
        }
        trim();
    }

    largeNumber_128 operator*(int v) const {
        largeNumber_128 res = *this;
        res *= v;
        return res;
    }

    friend pair<largeNumber_128, largeNumber_128> divmod(const largeNumber_128& a1, const largeNumber_128& b1) {
        int norm = base / (b1.a.back() + 1);
        largeNumber_128 a = a1.abs() * norm;
        largeNumber_128 b = b1.abs() * norm;
        largeNumber_128 q, r;
        q.a.resize(a.a.size());

        for (int i = a.a.size() - 1; i >= 0; i--) {
            r *= base;
            r += a.a[i];
            int s1 = r.a.size() <= b.a.size() ? 0 : r.a[b.a.size()];
            int s2 = r.a.size() <= b.a.size() - 1 ? 0 : r.a[b.a.size() - 1];
            int d = ((long long)base * s1 + s2) / b.a.back();
            r -= b * d;
            while (r < 0)
                r += b, --d;
            q.a[i] = d;
        }

        q.sign = a1.sign * b1.sign;
        r.sign = a1.sign;
        q.trim();
        r.trim();
        return make_pair(q, r / norm);
    }

    largeNumber_128 operator/(const largeNumber_128& v) const {
        return divmod(*this, v).first;
    }

    largeNumber_128 operator%(const largeNumber_128& v) const {
        return divmod(*this, v).second;
    }

    void operator/=(int v) {
        if (v < 0)
            sign = -sign, v = -v;
        for (int i = (int)a.size() - 1, rem = 0; i >= 0; --i) {
            long long cur = a[i] + rem * (long long)base;
            a[i] = (int)(cur / v);
            rem = (int)(cur % v);
        }
        trim();
    }

    largeNumber_128 operator/(int v) const {
        largeNumber_128 res = *this;
        res /= v;
        return res;
    }

    int operator%(int v) const {
        if (v < 0)
            v = -v;
        int m = 0;
        for (int i = a.size() - 1; i >= 0; --i)
            m = (a[i] + m * (long long)base) % v;
        return m * sign;
    }

    void operator+=(const largeNumber_128& v) {
        *this = *this + v;
    }
    void operator-=(const largeNumber_128& v) {
        *this = *this - v;
    }
    void operator*=(const largeNumber_128& v) {
        *this = *this * v;
    }
    void operator/=(const largeNumber_128& v) {
        *this = *this / v;
    }

    bool operator<(const largeNumber_128& v) const {
        if (sign != v.sign)
            return sign < v.sign;
        if (a.size() != v.a.size())
            return a.size() * sign < v.a.size() * v.sign;
        for (int i = a.size() - 1; i >= 0; i--)
            if (a[i] != v.a[i])
                return a[i] * sign < v.a[i] * sign;
        return false;
    }

    bool operator>(const largeNumber_128& v) const {
        return v < *this;
    }
    bool operator<=(const largeNumber_128& v) const {
        return !(v < *this);
    }
    bool operator>=(const largeNumber_128& v) const {
        return !(*this < v);
    }
    bool operator==(const largeNumber_128& v) const {
        return !(*this < v) && !(v < *this);
    }
    bool operator!=(const largeNumber_128& v) const {
        return *this < v || v < *this;
    }

    void trim() {
        while (!a.empty() && !a.back())
            a.pop_back();
        if (a.empty())
            sign = 1;
    }

    bool isZero() const {
        return a.empty() || (a.size() == 1 && !a[0]);
    }

    largeNumber_128 operator-() const {
        largeNumber_128 res = *this;
        res.sign = -sign;
        return res;
    }

    largeNumber_128 abs() const {
        largeNumber_128 res = *this;
        res.sign *= res.sign;
        return res;
    }

    long long longValue() const {
        long long res = 0;
        for (int i = a.size() - 1; i >= 0; i--)
            res = res * base + a[i];
        return res * sign;
    }

    friend largeNumber_128 gcd(const largeNumber_128& a, const largeNumber_128& b) {
        return b.isZero() ? a : gcd(b, a % b);
    }
    friend largeNumber_128 lcm(const largeNumber_128& a, const largeNumber_128& b) {
        return a / gcd(a, b) * b;
    }

    void read(const string& s) {
        sign = 1;
        a.clear();
        int pos = 0;
        while (pos < (int)s.size() && (s[pos] == '-' || s[pos] == '+')) {
            if (s[pos] == '-')
                sign = -sign;
            ++pos;
        }
        for (int i = s.size() - 1; i >= pos; i -= base_digits) {
            int x = 0;
            for (int j = max(pos, i - base_digits + 1); j <= i; j++)
                x = x * 10 + s[j] - '0';
            a.push_back(x);
        }
        trim();
    }

    friend istream& operator>>(istream& stream, largeNumber_128& v) {
        string s;
        stream >> s;
        v.read(s);
        return stream;
    }

    friend ostream& operator<<(ostream& stream, const largeNumber_128& v) {
        if (v.sign == -1)
            stream << '-';
        stream << (v.a.empty() ? 0 : v.a.back());
        for (int i = (int)v.a.size() - 2; i >= 0; --i)
            stream << setw(base_digits) << setfill('0') << v.a[i];
        return stream;
    }

    static vector<int> convert_base(const vector<int>& a, int old_digits, int new_digits) {
        vector<long long> p(max(old_digits, new_digits) + 1);
        p[0] = 1;
        for (int i = 1; i < (int)p.size(); i++)
            p[i] = p[i - 1] * 10;
        vector<int> res;
        long long cur = 0;
        int cur_digits = 0;
        for (int i = 0; i < (int)a.size(); i++) {
            cur += a[i] * p[cur_digits];
            cur_digits += old_digits;
            while (cur_digits >= new_digits) {
                res.push_back(int(cur % p[new_digits]));
                cur /= p[new_digits];
                cur_digits -= new_digits;
            }
        }
        res.push_back((int)cur);
        while (!res.empty() && !res.back())
            res.pop_back();
        return res;
    }

    typedef vector<long long> vll;

    static vll karatsubaMultiply(const vll& a, const vll& b) {
        int n = a.size();
        vll res(n + n);
        if (n <= 32) {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    res[i + j] += a[i] * b[j];
            return res;
        }

        int k = n >> 1;
        vll a1(a.begin(), a.begin() + k);
        vll a2(a.begin() + k, a.end());
        vll b1(b.begin(), b.begin() + k);
        vll b2(b.begin() + k, b.end());

        vll a1b1 = karatsubaMultiply(a1, b1);
        vll a2b2 = karatsubaMultiply(a2, b2);

        for (int i = 0; i < k; i++)
            a2[i] += a1[i];
        for (int i = 0; i < k; i++)
            b2[i] += b1[i];

        vll r = karatsubaMultiply(a2, b2);
        for (int i = 0; i < (int)a1b1.size(); i++)
            r[i] -= a1b1[i];
        for (int i = 0; i < (int)a2b2.size(); i++)
            r[i] -= a2b2[i];

        for (int i = 0; i < (int)r.size(); i++)
            res[i + k] += r[i];
        for (int i = 0; i < (int)a1b1.size(); i++)
            res[i] += a1b1[i];
        for (int i = 0; i < (int)a2b2.size(); i++)
            res[i + n] += a2b2[i];
        return res;
    }

    largeNumber_128 operator*(const largeNumber_128& v) const {
        vector<int> a6 = convert_base(this->a, base_digits, 6);
        vector<int> b6 = convert_base(v.a, base_digits, 6);
        vll a(a6.begin(), a6.end());
        vll b(b6.begin(), b6.end());
        while (a.size() < b.size())
            a.push_back(0);
        while (b.size() < a.size())
            b.push_back(0);
        while (a.size() & (a.size() - 1))
            a.push_back(0), b.push_back(0);
        vll c = karatsubaMultiply(a, b);
        largeNumber_128 res;
        res.sign = sign * v.sign;
        for (int i = 0, carry = 0; i < (int)c.size(); i++) {
            long long cur = c[i] + carry;
            res.a.push_back((int)(cur % 1000000));
            carry = (int)(cur / 1000000);
        }
        res.a = convert_base(res.a, 6, base_digits);
        res.trim();
        return res;
    }
};
struct UInt128 {
    uint64_t low;
    uint64_t high;
};

UInt128 createRandomLargeNumber() {
    UInt128 result;
    result.low = 0;
    result.high = 0;

    // Tạo ngẫu nhiên 128 bit (64 bit thấp và 64 bit cao)
    for (int i = 0; i < 88; ++i) {
        result.low = (result.low << 1) | (rand() & 1);
        if (i >= 43) {
            result.high = (result.high << 1) | (rand() & 1);
        }
    }
    return result;
}
// Hàm tính lũy thừa modul a^x mod p
largeNumber_128 Compute(const largeNumber_128& a, const largeNumber_128& x, const largeNumber_128& p) {
    largeNumber_128 result = 1;
    largeNumber_128 base = a % p;
    largeNumber_128 exponent = x;

    while (!exponent.isZero()) {
        if (exponent % 2 == 1) {
            result = (result * base) % p;
        }
        base = (base * base) % p;
        exponent = exponent / 2;
    }

    return result;
}
bool primalityCheck(largeNumber_128 num) {
    int k = 0;
    largeNumber_128 t = num - 1;
    UInt128 ua = createRandomLargeNumber();
    string high_a = to_string(ua.high);
    string low_a = to_string(ua.low);
    string resultStr_a = high_a + low_a;
    largeNumber_128 a;
    a.read(resultStr_a);
    a = a % num;
    while (t % 2 == 0) {
        k += 1;
        t /= 2;
    }
    largeNumber_128 x = Compute(a, t, num);
    if (x == 1 || x == num - 1)
        return true;
    for (int j = 1; j < k; j++) {
        x = Compute(x, 2, num);
        if (x == num - 1)
            return true;
    }
    return false;
}
string primalityCheck_result(largeNumber_128 num) {
    int count = 0;
    for (int i = 0; i < 50; i++) {
        if (primalityCheck(num) == true)
            count++;
    }
    if (count == 50) 
        return "possible prime";
    return "not prime";
}
int main() {
    // Khởi tạo bộ tạo số ngẫu nhiên dựa trên thời gian
    srand(static_cast<unsigned>(time(nullptr)));

    UInt128 random1 = createRandomLargeNumber();

    // Chuyển largeNumber.high và largeNumber.low thành chuỗi
    string highStr1 = to_string(random1.high);
    string lowStr1 = to_string(random1.low);

    // Ghép chuỗi
    string resultStr1 = highStr1 + lowStr1;

    cout << "Num1 (string) = " << resultStr1 << endl;

    UInt128 random2 = createRandomLargeNumber();

    // Chuyển largeNumber.high và largeNumber.low thành chuỗi
    string highStr2 = to_string(random2.high);
    string lowStr2 = to_string(random2.low);

    // Ghép chuỗi
    string resultStr2 = highStr2 + lowStr2;

    cout << "Num2 (string) = " << resultStr2 << endl;

    largeNumber_128 num1, num2;
    num1.read(resultStr1);
    num2.read(resultStr2);
    cout << "Num 1 = " << num1 << endl << "Num 2 = " << num2 << endl;
    largeNumber_128 GCD;
    GCD = gcd(num1, num2);
    cout << "GCD = " << GCD << endl;

    //--------YÊU CẦU 3----------
    largeNumber_128 num3, num4, num5;

    cout << "-----Nhap theo cong thuc a^x mod p-----" << endl;
    cout << "Nhap a: ";
    cin >> num3;
    cout << "Nhap x: ";
    cin >> num4;
    cout << "Nhap p: ";
    cin >> num5;
    cout << "Ket qua cua a^x mod p = " << Compute(num3, num4, num5) << endl;

    //---------YÊU CẦU 4--------------
    
    UInt128 random6 = createRandomLargeNumber();
    string highStr6 = to_string(random6.high);
    string lowStr6 = to_string(random6.low);
    string resultStr6 = highStr6 + lowStr6;
    largeNumber_128 num6;
    num6.read(resultStr6);
    if (num6 % 2 == 0)
        num6 -= 1;
    cout << "Num 6 = " << num6 << endl;
    cout << "Num 6 is " << primalityCheck_result(num6) << endl;
    return 0;
}
