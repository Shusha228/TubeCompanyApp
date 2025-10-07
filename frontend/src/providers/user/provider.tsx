import { getURL } from "@/api";
import type { User } from "@/models/user";
import { UserRole } from "@/models/user-role";
import WebApp from "@twa-dev/sdk";
import { useEffect, useState, type JSX } from "react";
import { UserContext } from "./context";

const _ADMIN_ARRAY_IN_STRING: string | undefined = import.meta.env.VITE_ADMINS;
const ADMIN_ARRAY =
  _ADMIN_ARRAY_IN_STRING !== undefined
    ? _ADMIN_ARRAY_IN_STRING.split(", ").map((el) => Number(el))
    : [];

console.log(ADMIN_ARRAY);
export const UserProvider = ({ children }: { children: JSX.Element }) => {
  const _user = WebApp.initDataUnsafe.user;
  const [user, setUser] = useState<User>({
    telegramId: _user !== undefined ? _user.id : 0,
    name: [_user?.first_name, _user?.last_name]
      .filter((el) => el !== undefined)
      .join(" "),
    role: UserRole.Admin,
    photo: _user?.photo_url,
  });

  useEffect(() => {
    if (user !== undefined) {
      fetch(getURL(`TelegramUsers/${user.telegramId}`)).then((el) => {
        if (el.status !== 200) {
          fetch(getURL(`TelegramUsers/`), {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              telegramUserId: user?.telegramId,
              firstName: _user?.first_name,
              lastName: _user?.last_name,
              username: _user?.username,
            }),
          });
        } else {
          if ((el.json() as { result?: { data?: User } }).result === undefined)
            return;
          const data = (el.json() as { result?: { data?: User } }).result?.data;
          setUser((u) => ({
            ...u,
            inn: data !== undefined ? data["inn"] : "",
            phone: data !== undefined ? data["phone"] : "",
            email: data !== undefined ? data["email"] : "",
          }));
        }
      });
    }
  }, [_user]);

  return <UserContext.Provider value={user}>{children}</UserContext.Provider>;
};
