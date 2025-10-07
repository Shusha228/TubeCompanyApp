import { getURL } from "@/api";
import type { User } from "@/models/user";
import { UserRole } from "@/models/user-role";
import WebApp from "@twa-dev/sdk";
import { useEffect, useState, type JSX } from "react";
import { UserContext } from "./context";

export const UserProvider = ({ children }: { children: JSX.Element }) => {
  const _user = WebApp.initDataUnsafe.user;
  const [user, setUser] = useState<User>({
    telegramId: _user !== undefined ? _user.id : 1,
    name: [_user?.first_name, _user?.last_name]
      .filter((el) => el !== undefined)
      .join(" "),
    role: UserRole.User,
    photo: _user?.photo_url,
  });

  useEffect(() => {
    if (user !== undefined) {
      fetch(getURL(`TelegramUsers/${user.telegramId}`))
        .then((el) => el.json())
        .then((res: User) => {
          setUser((e) => ({
            ...e,
            inn: res.inn,
          }));
        })
        .catch(() => {
          fetch(getURL(`TelegramUsers/`), {
            method: "POST",
            body: JSON.stringify({
              telegramUserId: _user?.id,
              firstName: _user?.first_name,
              lastName: _user?.last_name,
              username: _user?.username,
            }),
          });
        });
    }
  }, [user, _user]);

  return <UserContext.Provider value={user}>{children}</UserContext.Provider>;
};
