import { getURL } from "@/api";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useActiveModal } from "@/providers/active-modal";
import { useCreateOrder } from "@/providers/create-order";
import { useUser } from "@/providers/user";
import { useMemo, useState } from "react";

export const CreateOrderModal = () => {
  const user = useUser();
  const { item } = useCreateOrder();
  const { closeModal } = useActiveModal();

  const initialNameParts = useMemo(() => {
    const parts = (user?.name || "").trim().split(" ").filter(Boolean);
    return {
      lastName: parts[0] || "",
      firstName: parts[1] || (parts[0] ? "" : user?.name || ""),
      middleName: parts[2] || "",
    };
  }, [user?.name]);

  const [lastName, setLastName] = useState<string>(initialNameParts.lastName);
  const [firstName, setFirstName] = useState<string>(
    initialNameParts.firstName
  );
  const [middleName, setMiddleName] = useState<string>(
    initialNameParts.middleName
  );
  const [inn, setInn] = useState<string>(user?.inn || "");
  const [phone, setPhone] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [consent, setConsent] = useState<boolean>(false);

  const onSubmit = () => {
    if (!consent) return;
    const nowIso = new Date().toISOString();
    const body = {
      telegramUserId: user?.telegramId ?? 0,
      items: [item],
      customerInfo: {
        id: 0,
        userId: user?.telegramId ?? 0,
        firstName,
        lastName,
        inn,
        phone,
        email,
        createdAt: nowIso,
        updatedAt: nowIso,
      },
    };
    fetch(getURL("Orders"), {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(body),
    })
      .then(() => {})
      .catch(() => {})
      .finally(() => closeModal());
    console.log("CreateOrder body", body);
  };

  return (
    <div className="w-full h-auto">
      <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
        <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
          <h1 className="font-bold text-xl text-[#686868] text-center w-full">
            Оформление заказа
          </h1>
          <div className="flex w-full gap-2 px-2 md:px-4"></div>
        </div>
        <div className="w-full h-[10px]"></div>
        <div className="pb-8 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5 px-2">
          <div className="grid grid-cols-1 gap-2 px-2 md:px-4">
            <Label className="font-bold text-md">Контактные данные</Label>
            <Input
              required
              placeholder="Фамилия*"
              value={lastName}
              onChange={(e) => setLastName(e.currentTarget.value)}
            />
            <Input
              required
              placeholder="Имя*"
              value={firstName}
              onChange={(e) => setFirstName(e.currentTarget.value)}
            />
            <Input
              required
              placeholder="Отчество*"
              value={middleName}
              onChange={(e) => setMiddleName(e.currentTarget.value)}
            />
            <Input
              required
              placeholder="ИНН*"
              defaultValue={user.inn}
              value={inn}
              onChange={(e) => setInn(e.currentTarget.value)}
            />
            <Input
              required
              placeholder="Номер телефона*"
              defaultValue={user.phone}
              value={phone}
              onChange={(e) => setPhone(e.currentTarget.value)}
            />
            <Input
              required
              placeholder="Электронная почта*"
              value={email}
              defaultValue={user.email}
              onChange={(e) => setEmail(e.currentTarget.value)}
            />
            <div
              className="flex items-start gap-2 pt-4"
              onClick={() => setConsent((el) => !el)}
            >
              <Checkbox
                checked={consent}
                onCheckedChange={(v) => setConsent(Boolean(v))}
              />
              <Label className="text-sm text-[#686868]">
                Я даю свое согласие на обработку моих персональных данных в
                соответствии с законом Nº152-03 «0 персональных данных» от
                27.07.2006., согласно прилагаемой форме и Политике Компании в
                области защиты персональных данных.
              </Label>
            </div>
            <Button
              className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98"
              onClick={onSubmit}
            >
              Отправить
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};
