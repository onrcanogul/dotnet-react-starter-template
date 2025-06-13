import React, { useState } from "react";
import { Menu, Container, Icon, Sidebar, Segment } from "semantic-ui-react";
import theme from "../../../utils/theme";
import { useSelector } from "react-redux";
import { RootState } from "../../../store";

const HeaderComponent: React.FC = () => {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  // Redux'tan kullanıcı bilgilerini al
  const user = useSelector((state: RootState) => state.auth.user);
  const isAuthenticated = useSelector(
    (state: RootState) => state.auth.isAuthenticated
  );

  return (
    <>
      <Menu
        fixed="top"
        inverted
        size="large"
        style={{
          background: `linear-gradient(90deg, ${theme.colors.teal}, ${theme.colors.primary})`,
          padding: theme.spacing.padding,
        }}
      >
        <Container>
          <Menu.Item header>
            <Icon
              name="globe"
              size="large"
              style={{ color: theme.colors.text }}
            />
            <span style={{ color: theme.colors.text }}>
              {isAuthenticated && user
                ? `Template Project - ${user.username}`
                : "Template Project"}
            </span>
          </Menu.Item>

          <Menu.Item
            position="right"
            onClick={() => setSidebarOpen(!sidebarOpen)}
          >
            <Icon
              name="bars"
              size="large"
              style={{ color: theme.colors.text }}
            />
          </Menu.Item>
        </Container>
      </Menu>

      <Sidebar.Pushable
        as={Segment}
        style={{ marginTop: 50, background: "transparent" }}
      >
        <Sidebar
          as={Menu}
          animation="overlay"
          icon="labeled"
          inverted
          vertical
          visible={sidebarOpen}
          width="thin"
          style={{ backgroundColor: theme.colors.sidebarBackground }}
        >
          <Menu.Item as="a">
            <Icon name="home" />
            Ana Sayfa
          </Menu.Item>
          <Menu.Item as="a">
            <Icon name="info circle" />
            Hakkında
          </Menu.Item>
          <Menu.Item as="a">
            <Icon name="cogs" />
            Ayarlar
          </Menu.Item>
        </Sidebar>
      </Sidebar.Pushable>
    </>
  );
};

export default HeaderComponent;
