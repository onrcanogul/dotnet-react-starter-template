import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import {
  Container,
  Header,
  Icon,
  Button,
  Segment,
  Transition,
  Message,
} from "semantic-ui-react";

const Home: React.FC = () => {
  const { t, i18n } = useTranslation();
  const [visible, setVisible] = useState<boolean>(true);
  const navigate = useNavigate();

  return (
    <Container
      text
      textAlign="center"
      className="custom-container"
      style={{ marginTop: "100px" }}
    >
      <Transition animation="fade up" duration={1000} visible={visible}>
        <Segment
          raised
          padded="very"
          textAlign="center"
          style={{ boxShadow: "0px 4px 20px rgba(0,0,0,0.1)" }}
        >
          <Header as="h1" icon color="blue">
            <Icon name="globe" size="big" />
            {t("welcome")}
            <Header.Subheader
              style={{
                marginTop: "10px",
                fontSize: "1.2rem",
                fontWeight: "normal",
              }}
            >
              {t("home_subtitle")}
            </Header.Subheader>
          </Header>
        </Segment>
      </Transition>

      <Transition animation="scale" duration={800} visible={visible}>
        <Message color="blue" style={{ marginTop: "20px" }}>
          🌎 {t("change_language_info")}
        </Message>
      </Transition>

      <Button
        color="teal"
        size="large"
        animated="fade"
        onClick={() => {
          setVisible(false); // Önce mesajı kapat
          setTimeout(() => {
            i18n.changeLanguage(i18n.language === "en" ? "tr" : "en");
            setVisible(true); // Sonra tekrar aç
          }, 800);
        }}
        style={{ marginTop: "20px" }}
      >
        <Button.Content visible>{t("change_language")}</Button.Content>
        <Button.Content hidden>
          <Icon name="exchange" />
        </Button.Content>
      </Button>
      <Button
        color="teal"
        size="large"
        animated="fade"
        onClick={() => navigate("/auth")}
        style={{ marginTop: "20px" }}
      >
        <Button.Content visible>{t("authentication")}</Button.Content>
        <Button.Content hidden>
          <Icon name="unlock" />
        </Button.Content>
      </Button>
    </Container>
  );
};

export default Home;
